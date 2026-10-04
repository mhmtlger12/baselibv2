using System.Globalization;
using System.Text.Json;
using BaseLib.Presentation.Models.Site;
namespace BaseLib.Presentation.Services.Site;
public sealed class MockSiteContentService : ISiteContentService
{
    private static readonly CompareInfo Turkish = CultureInfo.GetCultureInfo("tr-TR").CompareInfo;
    public SiteContent Content { get; }
    public MockSiteContentService(IWebHostEnvironment environment)
    {
        Content = JsonSerializer.Deserialize<SiteContent>(File.ReadAllText(Path.Combine(environment.ContentRootPath, "Data", "site.json")), new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
    }
    public DepartmentPage? Departments(string category, string? level, string? query)
    {
        var card = Content.ScoreCards.Find(x => x.Key == category);
        if (card is null) return null;
        level = Content.LevelTabs.Any(x => x.Key == level) ? level : "lisans";
        return new(card, level!, query ?? "", Content.Departments.Where(x => x.Level == level && Contains(x.Name, query)).ToList());
    }
    public ScoreDetailPage? Detail(string category, int departmentId, string? period, string? institution, string? city, int page)
    {
        var card = Content.ScoreCards.Find(x => x.Key == category);
        var department = Content.Departments.Find(x => x.Id == departmentId);
        if (card is null || department is null) return null;
        period = Content.DetailPeriods.Contains(period ?? "") ? period : Content.DetailPeriods[0];
        var rows = Content.ScoreRows.Where(x => Contains(x.Institution, institution) && Contains(x.City, city)).ToList();
        var pages = Math.Max(1, (int)Math.Ceiling(rows.Count / 5d));
        page = Math.Clamp(page, 1, pages);
        return new(card, department, period!, institution ?? "", city ?? "", page, pages, rows.Count,
            rows.Sum(x => x.Quota), rows.Count == 0 ? null : rows.Min(x => x.MinScore), rows.Count == 0 ? null : rows.Max(x => x.MaxScore), rows.Skip((page - 1) * 5).Take(5).ToList());
    }
    public JobsPage Jobs(string? category, string? query, IReadOnlyList<JobListing> listings)
    {
        var categories = Content.JobCategories.SelectMany(x => new[] { x }.Concat(x.Children ?? [])).ToArray();
        var node = categories.FirstOrDefault(x => x.Key == category);
        var counts = categories.ToDictionary(x => x.Key, x => listings.Count(job => MatchesCategory(x, job)));
        var jobs = listings.Where(x => MatchesCategory(node, x) && Contains(x.Institution + " " + x.Summary, query))
            .OrderByDescending(x => x.PublishedAt).ThenBy(x => x.Id).ToArray();
        return new(node?.Key ?? "all", query ?? "", node?.Label ?? "Tüm İlanlar", jobs, Content.JobCategories, counts);
    }
    public SearchPage Search(string? query, IReadOnlyList<JobListing> listings) => new(query ?? "", Content.ScoreCards.Where(x => Contains(x.Title, query)).ToList(),
        Content.Departments.Where(x => Contains(x.Name, query)).ToList(), listings.Where(x => Contains(x.Institution + " " + x.Summary, query)).ToArray());

    private static bool MatchesCategory(JobCategory? category, JobListing job) =>
        category is null || category.Key == "all" || category.Key == job.CategoryKey ||
        category.Children?.Any(child => MatchesCategory(child, job)) == true;

    private static bool Contains(string value, string? query) => string.IsNullOrWhiteSpace(query) || Turkish.IndexOf(value, query.Trim(), CompareOptions.IgnoreCase) >= 0;
}
