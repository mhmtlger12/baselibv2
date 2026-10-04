using Baselib.Business.DTOs;
using Baselib.Core.Results;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IAuthService
{
    Task<IDataResult<AuthResultDto>> LoginAsync(LoginDto dto, ClientSessionInfoDto clientSession, CancellationToken cancellationToken = default);
    Task<IDataResult<AuthResultDto>> RefreshTokenAsync(string refreshToken, ClientSessionInfoDto clientSession, CancellationToken cancellationToken = default);
    Task<IResult> LogoutAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    Task<IDataResult<AuthResultDto>> SwitchRoleAsync(ClaimsPrincipal principal, int newRoleId, ClientSessionInfoDto clientSession, CancellationToken cancellationToken = default);
}
