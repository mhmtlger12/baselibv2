using Baselib.Business.DTOs;
using Baselib.Core.Results;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IAuthService
{
    Task<IDataResult<AuthResultDto>> LoginAsync(LoginDto dto);
    Task<IDataResult<AuthResultDto>> RefreshTokenAsync(string refreshToken);
    Task<IResult> LogoutAsync(ClaimsPrincipal principal);
    Task<IDataResult<AuthResultDto>> SwitchRoleAsync(ClaimsPrincipal principal, int newRoleId);
}
