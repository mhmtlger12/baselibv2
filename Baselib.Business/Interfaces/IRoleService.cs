using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IRoleService
{
    Task<IDataResult<IEnumerable<RoleDto>>> GetAllAsync();
    Task<IDataResult<RoleDto>> GetByIdAsync(int id);
    Task<IDataResult<RoleDto>> CreateAsync(CreateRoleDto dto);
    Task<IResult> UpdateAsync(int id, UpdateRoleDto dto);
    Task<IResult> DeleteAsync(int id);
    Task<IResult> AssignPermissionsAsync(int roleId, List<int> permissionIds);
    Task<IDataResult<IEnumerable<PermissionGroupDto>>> GetPermissionsByRoleIdAsync(int roleId);
    Task<IResult> UpdateWithPermissionsAsync(int id, UpdateRoleDto dto, List<PermissionGroupDto> permissionGroups);
}