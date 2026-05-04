using Baselib.Entities;

namespace Baselib.Business.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdWithPermissionsAsync(int id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> AddAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddRolePermissionAsync(RolePermission rolePermission);
    Task RemoveRolePermissionsAsync(int roleId);
}