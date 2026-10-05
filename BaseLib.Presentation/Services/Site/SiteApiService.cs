using System.Globalization;
using Baselib.Business.DTOs;
using BaseLib.Presentation.Models.Site;
using BaseLib.Presentation.Services.Api;
namespace BaseLib.Presentation.Services.Site;

public sealed class SiteApiService(ApiTransport transport) : ISiteContentService
{
    private static readonly CompareInfo Turkish = CultureInfo.GetCultureInfo("tr-TR").CompareInfo;
    private Task<SiteContent>? content;
    // Request-scoped: public partials share one response; the next request reads fresh data.
    public Task<SiteContent> GetAsync(CancellationToken ct) => content ??= transport.SendAsync<SiteContent>(HttpMethod.Get, "api/site", cancellationToken: ct);

    public async Task<DepartmentPage?> DepartmentsAsync(string category, string? level, string? query, CancellationToken ct)
    {
        var data = await GetAsync(ct);
        var card = data.ScoreCards.Find(x => x.Key == category);
        if (card is null) return null;
        level = data.LevelTabs.Any(x => x.Key == level) ? level : data.LevelTabs.FirstOrDefault()?.Key ?? "";
        return new(card, level!, query ?? "", data.Departments.Where(x => x.Level == level && Contains(x.Name, query)).ToList());
    }

    public async Task<ScoreDetailPage?> DetailAsync(string category, int departmentId, string? period, string? institution, string? city, int page, CancellationToken ct)
    {
        var data = await GetAsync(ct);
        var card = data.ScoreCards.Find(x => x.Key == category);
        var department = data.Departments.Find(x => x.Id == departmentId);
        if (card is null || department is null) return null;
        try
        {
            var scores = await transport.SendAsync<PublicScoresDto>(HttpMethod.Get,
                $"api/site/scores?category={Escape(category)}&programId={departmentId}&period={Escape(period)}&institution={Escape(institution)}&city={Escape(city)}&page={page}", cancellationToken: ct);
            var comments = await transport.SendAsync<List<SiteComment>>(HttpMethod.Get,
                "api/site/comments?page=" + Escape($"/taban-puanlari/{category}/bolum/{departmentId}"), cancellationToken: ct);
            return new(card, department, scores.Period, institution ?? "", city ?? "", scores.Page, scores.PageCount, scores.TotalCount,
                scores.TotalQuota, scores.MinScore, scores.MaxScore, scores.Rows, data.Periods.Select(x => x.Name).ToList(), comments);
        }
        catch (ApiException error) when (error.StatusCode == 404) { return null; }
    }
    public JobsPage Jobs(string? category, string? query, IReadOnlyList<JobListing> listings, IReadOnlyList<Baselib.Business.DTOs.JobCategoryDto> categoryOptions)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        List<JobCategory> BuildTree(string? parentKey) => categoryOptions.Where(x => x.ParentKey == parentKey)
            .OrderBy(x => x.SortOrder).Where(x => visited.Add(x.Key))
            .Select(x => new JobCategory(x.Key, x.Label, BuildTree(x.Key))).ToList();
        var tree = BuildTree(null);
        tree.Insert(0, new("all", "Tüm İlanlar", null));
        static IEnumerable<JobCategory> Flatten(IEnumerable<JobCategory> nodes) =>
            nodes.SelectMany(x => new[] { x }.Concat(Flatten(x.Children ?? [])));
        var categories = Flatten(tree).ToArray();
        var node = categories.FirstOrDefault(x => x.Key == category);
        var counts = categories.ToDictionary(x => x.Key, x => listings.Count(job => MatchesCategory(x, job)));
        var jobs = listings.Where(x => MatchesCategory(node, x) && Contains(x.Institution + " " + x.Summary, query))
            .OrderByDescending(x => x.PublishedAt).ThenBy(x => x.Id).ToArray();
        return new(node?.Key ?? "all", query ?? "", node?.Label ?? "Tüm İlanlar", jobs, tree, counts);
    }

    public async Task<SearchPage> SearchAsync(string? query, IReadOnlyList<JobListing> listings, CancellationToken ct)
    {
        var data = await GetAsync(ct);
        return new(query ?? "", data.ScoreCards.Where(x => Contains(x.Title, query)).ToList(),
            data.Departments.Where(x => Contains(x.Name, query)).ToList(), listings.Where(x => Contains(x.Institution + " " + x.Summary, query)).ToArray());
    }
    public async Task SendContactAsync(SendContactDto input, CancellationToken ct) =>
        await transport.SendAsync<object>(HttpMethod.Post, "api/site/contact", input, cancellationToken: ct);
    public async Task SendCommentAsync(SendCommentDto input, CancellationToken ct) =>
        await transport.SendAsync<object>(HttpMethod.Post, "api/site/comments", input, cancellationToken: ct);
    private static string Escape(string? value) => Uri.EscapeDataString(value ?? "");

    private static bool MatchesCategory(JobCategory? category, JobListing job) =>
        category is null || category.Key == "all" || category.Key == job.CategoryKey ||
        category.Children?.Any(child => MatchesCategory(child, job)) == true;

    private static bool Contains(string value, string? query) => string.IsNullOrWhiteSpace(query) || Turkish.IndexOf(value, query.Trim(), CompareOptions.IgnoreCase) >= 0;
}
