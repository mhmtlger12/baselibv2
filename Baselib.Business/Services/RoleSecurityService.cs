using System.Security.Claims;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Baselib.Core.Constants;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class RoleSecurityService(
    IRepository<Role> roles,
    IRepository<Permission> permissions,
    IRepository<RolePermission> rolePermissions,
    IRepository<UserRole> userRoles,
    IPermissionCheckService permissionCheck) : IRoleSecurityService
{
    public async Task<IDataResult<bool>> ValidateManagementAsync(
        ClaimsPrincipal principal, Role? role, IEnumerable<int> permissionIds)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return DataResult<bool>.Unauthorized();

        var ids = permissionIds.Distinct().ToArray();
        var selected = (await permissions.GetAllAsync(p => ids.Contains(p.Id), asNoTracking: true)).ToArray();
        if (selected.Length != ids.Length)
            return DataResult<bool>.BadRequest(Messages.Role.InvalidPermissionSelection);

        var privileged = selected.Any(p => SecurityPermissions.CriticalCodes.Contains(p.Code)) ||
            (role is not null && await AnyPrivilegedAsync([role.Id]));
        var granted = await GrantedCodesAsync(principal);
        if (privileged && !granted.Contains(Constants.Permissions.UsersAssignPrivilegedRoles))
            return DataResult<bool>.ErrorDataResult(Messages.Role.PrivilegedManagementNotAllowed, 403);
        if (selected.Any(p => !granted.Contains(p.Code)))
            return DataResult<bool>.ErrorDataResult(Messages.Role.PermissionDelegationNotAllowed, 403);

        return DataResult<bool>.Ok(privileged);
    }

    public async Task<IResult> ValidateAssignmentAsync(
        ClaimsPrincipal? principal, IEnumerable<int> roleIds, int? targetUserId = null)
    {
        var ids = roleIds.Distinct().ToArray();
        // Public registration creates a role-free account.
        if (ids.Length == 0 && targetUserId is null)
            return Result.Ok();
        if (principal?.Identity?.IsAuthenticated != true)
            return Result.Forbidden(Messages.User.RoleAssignmentNotAllowed);
        if (await roles.CountAsync(role => ids.Contains(role.Id)) != ids.Length)
            return Result.BadRequest(Messages.User.InvalidRoleSelection);

        var granted = await GrantedCodesAsync(principal);
        if (!granted.Contains(Constants.Permissions.UsersAssignRoles))
            return Result.Forbidden(Messages.User.RoleAssignmentNotAllowed);

        // Also protect removal of an existing privileged role (demote-then-reset bypass).
        var privileged = await AnyPrivilegedAsync(ids) ||
            (targetUserId.HasValue && await HasPrivilegedRoleAsync(targetUserId.Value));
        if (privileged && !granted.Contains(Constants.Permissions.UsersAssignPrivilegedRoles))
            return Result.Forbidden(Messages.User.PrivilegedRoleAssignmentNotAllowed);

        var requestedCodes = await rolePermissions.SelectAsync(query => query
            .Where(rp => ids.Contains(rp.RoleId)).Select(rp => rp.Permission.Code).Distinct());
        return requestedCodes.All(granted.Contains)
            ? Result.Ok()
            : Result.Forbidden(Messages.Role.PermissionDelegationNotAllowed);
    }

    public async Task<bool> HasPrivilegedRoleAsync(int userId)
    {
        var assigned = await userRoles.GetAllAsync(ur => ur.UserId == userId,
            ignoreQueryFilters: true, asNoTracking: true);
        return await AnyPrivilegedAsync(assigned.Select(ur => ur.RoleId).ToArray());
    }

    private async Task<bool> AnyPrivilegedAsync(int[] ids)
    {
        if (ids.Length == 0) return false;
        if (await roles.AnyAsync(role => ids.Contains(role.Id) && (role.IsPrivileged || role.IsSystemRole),
                ignoreQueryFilters: true)) return true;
        var criticalCodes = SecurityPermissions.CriticalCodes.ToArray();
        return await rolePermissions.AnyAsync(rp => ids.Contains(rp.RoleId) && criticalCodes.Contains(rp.Permission.Code),
            ignoreQueryFilters: true);
    }

    private Task<IReadOnlySet<string>> GrantedCodesAsync(ClaimsPrincipal principal) =>
        permissionCheck.GetGrantedCodesAsync(ClaimsPrincipalHelper.GetUserId(principal),
            ClaimsPrincipalHelper.GetActiveRoleId(principal));
}
