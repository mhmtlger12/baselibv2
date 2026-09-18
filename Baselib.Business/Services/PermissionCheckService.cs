using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class PermissionCheckService : IPermissionCheckService
{
    private readonly IRepository<UserRole> _userRoles;
    private readonly IRepository<Permission> _permissions;
    private readonly IRepository<RolePermission> _rolePermissions;

    public PermissionCheckService(
        IRepository<UserRole> userRoles,
        IRepository<Permission> permissions,
        IRepository<RolePermission> rolePermissions)
    {
        _userRoles = userRoles;
        _permissions = permissions;
        _rolePermissions = rolePermissions;
    }

    public async Task<bool> HasAccessAsync(int userId, int? activeRoleId, string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
            return false;

        // Devre dışı kullanıcılar ve roller, token süreleri henüz dolmamış olsa bile erişemez.
        var userRoles = await _userRoles.GetAllAsync(
            ur => ur.UserId == userId && ur.User.IsActive && ur.Role.IsActive);
        var currentUserRoleIds = userRoles.Select(ur => ur.RoleId).ToList();

        if (!currentUserRoleIds.Any())
            return false;

        List<int> effectiveRoleIds;

        if (activeRoleId.HasValue)
        {
            // JWT içindeki aktif rol veritabanında HALA mevcut mu? (Revoke kontrolü)
            if (currentUserRoleIds.Contains(activeRoleId.Value))
            {
                effectiveRoleIds = new List<int> { activeRoleId.Value };
            }
            else
            {
                // Rol geri alınmış ama token süresi bitmemiş. Erişimi reddet.
                return false;
            }
        }
        else
        {
            effectiveRoleIds = currentUserRoleIds;
        }

        if (!effectiveRoleIds.Any())
            return false;

        var permissions = await _permissions.GetAllAsync(p => p.Code == permissionCode);
        var permissionIds = permissions.Select(p => p.Id).ToList();

        // Tanımsız veya devre dışı bırakılmış kaynak, varsayılan olarak yasaktır.
        if (!permissionIds.Any())
            return false;

        return await _rolePermissions.AnyAsync(rp =>
            effectiveRoleIds.Contains(rp.RoleId) &&
            permissionIds.Contains(rp.PermissionId));
    }
}
