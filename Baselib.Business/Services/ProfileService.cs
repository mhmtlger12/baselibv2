using System.Security.Claims;
using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;

namespace Baselib.Business.Services;

public class ProfileService : IProfileService
{
    private readonly IUserService _userService;

    public ProfileService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<UserDto> GetMyProfileAsync(ClaimsPrincipal principal)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);
        var activeRoleId = ClaimsPrincipalHelper.GetActiveRoleId(principal);

        var userDto = await _userService.GetByIdAsync(userId, activeRoleId);

        if (userDto == null)
            throw new KeyNotFoundException(Core.Messages.Messages.User.NotFound);

        return userDto;
    }

    public async Task ChangeMyPasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);
        await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);
    }
}
