using Baselib.Business.DTOs;
using Baselib.Core.Results;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IUserService
{
    Task<IDataResult<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<UserDto>> GetByIdAsync(int id, int? activeRoleId = null, CancellationToken cancellationToken = default);
    Task<IDataResult<UserDto>> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);
    Task<IDataResult<UserDto>> CreateAsync(CreateUserDto dto, ClaimsPrincipal? principal = null, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<IResult> ResetPasswordAsync(ClaimsPrincipal principal, int id, ResetUserPasswordDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IResult> AssignRolesAsync(ClaimsPrincipal principal, int userId, List<int> roleIds, CancellationToken cancellationToken = default);
    Task<IResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task RevokeUserSessionsAsync(int userId, string reason, CancellationToken cancellationToken = default);
}
