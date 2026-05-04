using Baselib.Entities;

namespace Baselib.Business.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdWithRolesAsync(int id);
    Task<IEnumerable<User>> GetAllWithDepartmentAsync();
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
    Task AddUserRoleAsync(UserRole userRole);
    Task RemoveUserRolesAsync(int userId);
}