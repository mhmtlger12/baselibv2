using Baselib.Business.DTOs;
using Baselib.Core.Results;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IProfileService
{
    Task<IDataResult<UserDto>> GetMyProfileAsync(ClaimsPrincipal principal);
    Task<IResult> ChangeMyPasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword);
}
