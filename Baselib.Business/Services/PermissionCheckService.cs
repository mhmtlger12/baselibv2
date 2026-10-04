using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class PermissionCheckService(IRepository<RolePermission> rolePermissions) : IPermissionCheckService
{
    public async Task<bool> HasAccessAsync(int userId, int? activeRoleId, string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(permissionCode)) return false;
        return await rolePermissions.AnyAsync(rp => rp.Permission.Code == permissionCode &&
            rp.Permission.IsActive && rp.Role.IsActive &&
            (!activeRoleId.HasValue || rp.RoleId == activeRoleId.Value) &&
            rp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.User.IsActive));
    }

    public async Task<IReadOnlySet<string>> GetGrantedCodesAsync(int userId, int? activeRoleId)
    {
        var codes = await rolePermissions.SelectAsync(query => query.Where(rp =>
            rp.Permission.IsActive && rp.Role.IsActive &&
            (!activeRoleId.HasValue || rp.RoleId == activeRoleId.Value) &&
            rp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.User.IsActive))
            .Select(rp => rp.Permission.Code).Distinct());
        return codes.ToHashSet(StringComparer.Ordinal);
    }
}
