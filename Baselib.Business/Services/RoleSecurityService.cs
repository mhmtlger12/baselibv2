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
    IEntityRepository<Role> roles,
    IEntityRepository<Permission> permissions,
    IRepository<RolePermission> rolePermissions,
    IRepository<UserRole> userRoles,
    IPermissionCheckService permissionCheck) : IRoleSecurityService
{
    public async Task<IDataResult<bool>> ValidateManagementAsync(
        ClaimsPrincipal principal, Role? role, IEnumerable<int> permissionIds, CancellationToken cancellationToken = default)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return DataResult<bool>.Unauthorized();

        var ids = permissionIds.Distinct().ToArray();
        var selected = (await permissions.GetAllAsync(p => ids.Contains(p.Id), asNoTracking: true, cancellationToken: cancellationToken)).ToArray();
        if (selected.Length != ids.Length)
            return DataResult<bool>.BadRequest(Messages.Role.InvalidPermissionSelection);

        var privileged = selected.Any(p => SecurityPermissions.CriticalCodes.Contains(p.Code)) ||
            (role is not null && await AnyPrivilegedAsync([role.Id], cancellationToken: cancellationToken));
        var granted = await GrantedCodesAsync(principal, cancellationToken: cancellationToken);
        if (privileged && !granted.Contains(Constants.Permissions.UsersAssignPrivilegedRoles))
            return DataResult<bool>.ErrorDataResult(Messages.Role.PrivilegedManagementNotAllowed, 403);
        if (selected.Any(p => !granted.Contains(p.Code)))
            return DataResult<bool>.ErrorDataResult(Messages.Role.PermissionDelegationNotAllowed, 403);

        return DataResult<bool>.Ok(privileged);
    }

    public async Task<IResult> ValidateAssignmentAsync(
        ClaimsPrincipal? principal, IEnumerable<int> roleIds, int? targetUserId = null, CancellationToken cancellationToken = default)
    {
        var ids = roleIds.Distinct().ToArray();
        // Public registration creates a role-free account.
        if (ids.Length == 0 && targetUserId is null)
            return Result.Ok();
        if (principal?.Identity?.IsAuthenticated != true)
            return Result.Forbidden(Messages.User.RoleAssignmentNotAllowed);
        if (await roles.CountAsync(role => ids.Contains(role.Id), cancellationToken: cancellationToken) != ids.Length)
            return Result.BadRequest(Messages.User.InvalidRoleSelection);

        var granted = await GrantedCodesAsync(principal, cancellationToken: cancellationToken);
        if (!granted.Contains(Constants.Permissions.UsersAssignRoles))
            return Result.Forbidden(Messages.User.RoleAssignmentNotAllowed);

        // Also protect removal of an existing privileged role (demote-then-reset bypass).
        var privileged = await AnyPrivilegedAsync(ids, cancellationToken: cancellationToken) ||
            (targetUserId.HasValue && await HasPrivilegedRoleAsync(targetUserId.Value, cancellationToken: cancellationToken));
        if (privileged && !granted.Contains(Constants.Permissions.UsersAssignPrivilegedRoles))
            return Result.Forbidden(Messages.User.PrivilegedRoleAssignmentNotAllowed);

        var requestedCodes = await rolePermissions.SelectAsync(query => query
            .Where(rp => ids.Contains(rp.RoleId)).Select(rp => rp.Permission.Code).Distinct(), cancellationToken: cancellationToken);
        return requestedCodes.All(granted.Contains)
            ? Result.Ok()
            : Result.Forbidden(Messages.Role.PermissionDelegationNotAllowed);
    }

    public async Task<bool> HasPrivilegedRoleAsync(int userId, CancellationToken cancellationToken = default)
    {
        var assigned = await userRoles.GetAllAsync(ur => ur.UserId == userId,
            ignoreQueryFilters: true, asNoTracking: true, cancellationToken: cancellationToken);
        return await AnyPrivilegedAsync(assigned.Select(ur => ur.RoleId).ToArray(), cancellationToken: cancellationToken);
    }

    private async Task<bool> AnyPrivilegedAsync(int[] ids, CancellationToken cancellationToken = default)
    {
        if (ids.Length == 0) return false;
        if (await roles.AnyAsync(role => ids.Contains(role.Id) && (role.IsPrivileged || role.IsSystemRole),
                ignoreQueryFilters: true, cancellationToken: cancellationToken)) return true;
        var criticalCodes = SecurityPermissions.CriticalCodes.ToArray();
        return await rolePermissions.AnyAsync(rp => ids.Contains(rp.RoleId) && criticalCodes.Contains(rp.Permission.Code),
            ignoreQueryFilters: true, cancellationToken: cancellationToken);
    }

    private Task<IReadOnlySet<string>> GrantedCodesAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default) =>
        permissionCheck.GetGrantedCodesAsync(ClaimsPrincipalHelper.GetUserId(principal),
            ClaimsPrincipalHelper.GetActiveRoleId(principal), cancellationToken: cancellationToken);
}
