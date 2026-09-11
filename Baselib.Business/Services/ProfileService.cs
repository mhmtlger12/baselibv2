using System.Security.Claims;
using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Baselib.Core.Results;

namespace Baselib.Business.Services;

public class ProfileService : IProfileService
{
    private readonly IUserService _userService;

    public ProfileService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IDataResult<UserDto>> GetMyProfileAsync(ClaimsPrincipal principal)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);
        var activeRoleId = ClaimsPrincipalHelper.GetActiveRoleId(principal);

        return await _userService.GetByIdAsync(userId, activeRoleId);
    }

    public async Task<IResult> ChangeMyPasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);
        return await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);
    }
}
