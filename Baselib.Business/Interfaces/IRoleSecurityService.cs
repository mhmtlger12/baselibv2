using System.Security.Claims;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Interfaces;

public interface IRoleSecurityService
{
    Task<IDataResult<bool>> ValidateManagementAsync(ClaimsPrincipal principal, Role? role, IEnumerable<int> permissionIds, CancellationToken cancellationToken = default);
    Task<IResult> ValidateAssignmentAsync(ClaimsPrincipal? principal, IEnumerable<int> roleIds, int? targetUserId = null, CancellationToken cancellationToken = default);
    Task<bool> HasPrivilegedRoleAsync(int userId, CancellationToken cancellationToken = default);
}
