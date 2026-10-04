using System.Security.Claims;
using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IRoleService
{
    Task<IDataResult<IEnumerable<RoleDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<SelectOptionDto>>> GetSelectOptionsAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<RoleDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<RoleDto>> CreateAsync(ClaimsPrincipal principal, CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(ClaimsPrincipal principal, int id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(ClaimsPrincipal principal, int id, CancellationToken cancellationToken = default);
    Task<IResult> AssignPermissionsAsync(ClaimsPrincipal principal, int roleId, List<int> permissionIds, CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<PermissionGroupDto>>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
}