using Baselib.Business.DTOs;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(LoginDto dto);
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(ClaimsPrincipal principal);
    Task<AuthResultDto> SwitchRoleAsync(ClaimsPrincipal principal, int newRoleId);
}
