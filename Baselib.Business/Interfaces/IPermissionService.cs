using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDto>> GetAllAsync();
    Task<PermissionDto?> GetByIdAsync(int id);
    Task<PermissionDto> CreateAsync(CreatePermissionDto dto);
    Task UpdateAsync(int id, CreatePermissionDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<PermissionGroupDto>> GetGroupedPermissionsAsync(int? roleId = null);
    Task SaveRolePermissionsAsync(int roleId, List<PermissionGroupDto> permissionGroups);
}