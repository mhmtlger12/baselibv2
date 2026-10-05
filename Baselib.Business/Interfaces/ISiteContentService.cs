using Baselib.Business.DTOs;
using Baselib.Core.Results;
namespace Baselib.Business.Interfaces;

public interface ISiteContentService
{
    Task<IDataResult<SiteContentDto>> GetAsync(CancellationToken ct);
    Task<IDataResult<PublicScoresDto>> ScoresAsync(string category, int programId, string? period, string? institution, string? city, int page, CancellationToken ct);
    Task<IDataResult<IReadOnlyList<SiteCommentDto>>> CommentsAsync(string page, CancellationToken ct);
    Task<IResult> SendContactAsync(SendContactDto input, CancellationToken ct);
    Task<IResult> SendCommentAsync(SendCommentDto input, CancellationToken ct);
}
