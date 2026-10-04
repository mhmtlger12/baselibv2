using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IAuditLogService
{
    Task<IDataResult<IEnumerable<AuditLogDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task LogAsync(int? userId, string action, string controller, string route, string? details, CancellationToken cancellationToken = default);
}
