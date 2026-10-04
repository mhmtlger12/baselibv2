using System.Security.Claims;
using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IRoleService
{
    Task<IDataResult<IEnumerable<RoleDto>>> GetAllAsync();
    Task<IDataResult<IEnumerable<SelectOptionDto>>> GetSelectOptionsAsync();
    Task<IDataResult<RoleDto>> GetByIdAsync(int id);
    Task<IDataResult<RoleDto>> CreateAsync(ClaimsPrincipal principal, CreateRoleDto dto);
    Task<IResult> UpdateAsync(ClaimsPrincipal principal, int id, UpdateRoleDto dto);
    Task<IResult> DeleteAsync(ClaimsPrincipal principal, int id);
    Task<IResult> AssignPermissionsAsync(ClaimsPrincipal principal, int roleId, List<int> permissionIds);
    Task<IDataResult<IEnumerable<PermissionGroupDto>>> GetPermissionsByRoleIdAsync(int roleId);
}