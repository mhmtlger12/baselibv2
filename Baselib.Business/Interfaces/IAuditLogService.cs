using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogDto>> GetAllAsync();
    Task LogAsync(int? userId, string action, string controller, string route, string? details);
}
