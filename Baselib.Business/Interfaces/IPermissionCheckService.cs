namespace Baselib.Business.Interfaces;

/// <summary>
/// Api katmanının doğrudan Data katmanına erişmesini önlemek için
/// PermissionHandler'ın kullandığı yetki kontrol servisi.
/// </summary>
public interface IPermissionCheckService
{
    Task<bool> HasAccessAsync(int userId, int? activeRoleId, string controller, string action);
}
