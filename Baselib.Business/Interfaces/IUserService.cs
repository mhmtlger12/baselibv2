using Baselib.Business.DTOs;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id, int? activeRoleId = null);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task UpdateAsync(int id, UpdateUserDto dto);
    Task DeleteAsync(int id);
    Task<AuthResultDto> LoginAsync(LoginDto dto);
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(ClaimsPrincipal principal);
    Task AssignRolesAsync(int userId, List<int> roleIds);
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task ChangeMyPasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword);
    Task<AuthResultDto> SwitchRoleAsync(ClaimsPrincipal principal, int newRoleId);
    Task<UserDto> GetMyProfileAsync(ClaimsPrincipal principal);
}