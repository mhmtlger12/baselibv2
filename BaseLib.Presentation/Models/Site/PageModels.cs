namespace BaseLib.Presentation.Models.Site;
public sealed record HomePage(IReadOnlyList<Countdown> Countdowns, IReadOnlyList<ScoreCard> ScoreCards,
    IReadOnlyList<JobListing> JobListings);
public sealed record DepartmentPage(ScoreCard Card, string Level, string Query, IReadOnlyList<StudyDepartment> Departments);
public sealed record ScoreDetailPage(ScoreCard Card, StudyDepartment Department, string Period, string Institution,
    string City, int Page, int PageCount, int TotalCount, int TotalQuota, decimal? MinScore, decimal? MaxScore, IReadOnlyList<ScoreRow> Rows, IReadOnlyList<string> Periods, IReadOnlyList<SiteComment> Comments);
public sealed record JobsPage(string Category, string Query, string Title, IReadOnlyList<JobListing> Jobs,
    IReadOnlyList<JobCategory> Categories, IReadOnlyDictionary<string, int> CategoryCounts);
public sealed record SearchPage(string Query, IReadOnlyList<ScoreCard> Cards, IReadOnlyList<StudyDepartment> Departments, IReadOnlyList<JobListing> Jobs);
public sealed class ContactInput : Baselib.Business.DTOs.SendContactDto;
public sealed record CommentFormModel(string FormId, string Category, int ProgramId, int? ParentId = null);
public static class SiteStyle
{
    public static readonly string[] Pastels = ["border-teal-200 bg-teal-50", "border-navy-200 bg-navy-50", "border-sky-200 bg-sky-50", "border-cyan-200 bg-cyan-50", "border-amber-200 bg-amber-50"];
    public static string Initials(string name) => string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(x => x[0])).ToUpper(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"));
    public static string Score(decimal? score) => score?.ToString("N5") ?? "—";
}
