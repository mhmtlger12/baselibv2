using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(int id);
    Task<RoleDto> CreateAsync(CreateRoleDto dto);
    Task UpdateAsync(int id, UpdateRoleDto dto);
    Task DeleteAsync(int id);
    Task AssignPermissionsAsync(int roleId, List<int> permissionIds);
    Task<IEnumerable<PermissionGroupDto>> GetPermissionsByRoleIdAsync(int roleId);
    Task UpdateWithPermissionsAsync(int id, UpdateRoleDto dto, List<PermissionGroupDto> permissionGroups);
}