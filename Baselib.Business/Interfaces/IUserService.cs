using Baselib.Business.DTOs;
using Baselib.Core.Results;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IUserService
{
    Task<IDataResult<IEnumerable<UserDto>>> GetAllAsync();
    Task<IDataResult<UserDto>> GetByIdAsync(int id, int? activeRoleId = null);
    Task<IDataResult<UserDto>> RegisterAsync(RegisterUserDto dto);
    Task<IDataResult<UserDto>> CreateAsync(CreateUserDto dto, ClaimsPrincipal? principal = null);
    Task<IResult> UpdateAsync(int id, UpdateUserDto dto);
    Task<IResult> DeleteAsync(int id);
    Task<IResult> AssignRolesAsync(ClaimsPrincipal principal, int userId, List<int> roleIds);
    Task<IResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task RevokeUserSessionsAsync(int userId, string reason);
}
