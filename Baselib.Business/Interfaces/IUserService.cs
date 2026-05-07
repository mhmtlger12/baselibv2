using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id, int? activeRoleId = null);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task UpdateAsync(int id, UpdateUserDto dto);
    Task DeleteAsync(int id);
    Task AssignRolesAsync(int userId, List<int> roleIds);
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}