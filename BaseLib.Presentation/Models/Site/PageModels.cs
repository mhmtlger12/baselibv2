using System.ComponentModel.DataAnnotations;
namespace BaseLib.Presentation.Models.Site;
public sealed record DepartmentPage(ScoreCard Card, string Level, string Query, IReadOnlyList<StudyDepartment> Departments);
public sealed record ScoreDetailPage(ScoreCard Card, StudyDepartment Department, string Period, string Institution,
    string City, int Page, int PageCount, int TotalCount, int TotalQuota, decimal? MinScore, decimal? MaxScore, IReadOnlyList<ScoreRow> Rows);
public sealed record JobsPage(string Category, string Query, string Title, IReadOnlyList<JobListing> Jobs);
public sealed record SearchPage(string Query, IReadOnlyList<ScoreCard> Cards, IReadOnlyList<StudyDepartment> Departments, IReadOnlyList<JobListing> Jobs);
public sealed class ContactInput
{
    [Required(ErrorMessage = "Adınızı yazın."), StringLength(100)] public string Name { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, StringLength(200)] public string Subject { get; set; } = "";
    [Required, StringLength(4000)] public string Message { get; set; } = "";
}
public static class SiteStyle
{
    public static readonly string[] Pastels = ["border-teal-200 bg-teal-50", "border-navy-200 bg-navy-50", "border-sky-200 bg-sky-50", "border-cyan-200 bg-cyan-50", "border-amber-200 bg-amber-50"];
    public static string Initials(string name) => string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(x => x[0])).ToUpper(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"));
    public static string Score(decimal? score) => score?.ToString("N5") ?? "—";
}
