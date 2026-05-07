using Baselib.Business.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<bool> HasAccessAsync(int userId, int? activeRoleId, string controller, string action)
    {
        // Veritabanından kullanıcının GÜNCEL rollerini çekiyoruz (Güvenlik için şart)
        var currentUserRoleIds = await _userRoles.Query()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

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

        var permissionIds = await _permissions.Query()
            .Where(p =>
                p.ControllerName.ToUpper() == controller.ToUpper() &&
                p.ActionName.ToUpper() == action.ToUpper() &&
                p.IsActive)
            .Select(p => p.Id)
            .ToListAsync();

        // Bu controller/action için tanımlı permission yoksa erişime izin ver
        if (!permissionIds.Any())
            return true;

        return await _rolePermissions.Query()
            .AnyAsync(rp =>
                effectiveRoleIds.Contains(rp.RoleId) &&
                permissionIds.Contains(rp.PermissionId));
    }
}
