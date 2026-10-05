using Baselib.Business.DTOs;
using BaseLib.Presentation.Models.Site;
namespace BaseLib.Presentation.Services.Site;
public interface ISiteContentService
{
    Task<SiteContent> GetAsync(CancellationToken ct);
    Task<DepartmentPage?> DepartmentsAsync(string category, string? level, string? query, CancellationToken ct);
    Task<ScoreDetailPage?> DetailAsync(string category, int departmentId, string? period, string? institution, string? city, int page, CancellationToken ct);
    JobsPage Jobs(string? category, string? query, IReadOnlyList<JobListing> listings, IReadOnlyList<JobCategoryDto> categories);
    Task<SearchPage> SearchAsync(string? query, IReadOnlyList<JobListing> listings, CancellationToken ct);
    Task SendContactAsync(SendContactDto input, CancellationToken ct);
    Task SendCommentAsync(SendCommentDto input, CancellationToken ct);
}
