namespace Baselib.Business.Interfaces;

/// <summary>
/// Api katmanının doğrudan Data katmanına erişmesini önlemek için
/// PermissionHandler'ın kullandığı yetki kontrol servisi.
/// </summary>
public interface IPermissionCheckService
{
    Task<IReadOnlySet<string>> GetGrantedCodesAsync(int userId, int? activeRoleId, CancellationToken cancellationToken = default);
    Task<bool> HasAccessAsync(int userId, int? activeRoleId, string permissionCode, CancellationToken cancellationToken = default);
}
