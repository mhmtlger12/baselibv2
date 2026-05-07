using Baselib.Business.DTOs;
using System.Security.Claims;

namespace Baselib.Business.Interfaces;

public interface IProfileService
{
    Task<UserDto> GetMyProfileAsync(ClaimsPrincipal principal);
    Task ChangeMyPasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword);
}
