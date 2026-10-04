using Baselib.Business.DTOs;
using Baselib.Core.Results;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IProfileService
{
    Task<IDataResult<UserDto>> GetMyProfileAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    Task<IResult> ChangeMyPasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}
