using BaseLib.Presentation.Models.Site;
namespace BaseLib.Presentation.Services.Site;
public interface ISiteContentService
{
    SiteContent Content { get; }
    DepartmentPage? Departments(string category, string? level, string? query);
    ScoreDetailPage? Detail(string category, int departmentId, string? period, string? institution, string? city, int page);
    JobsPage Jobs(string? category, string? query);
    SearchPage Search(string? query);
}
