using System.Security.Claims;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Interfaces;

public interface IRoleSecurityService
{
    Task<IDataResult<bool>> ValidateManagementAsync(ClaimsPrincipal principal, Role? role, IEnumerable<int> permissionIds);
    Task<IResult> ValidateAssignmentAsync(ClaimsPrincipal? principal, IEnumerable<int> roleIds, int? targetUserId = null);
    Task<bool> HasPrivilegedRoleAsync(int userId);
}
