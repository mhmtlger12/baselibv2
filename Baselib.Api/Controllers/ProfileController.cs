using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = await _profileService.GetMyProfileAsync(User);
        return Ok(DataResult<UserDto>.SuccessDataResult(user));
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        await _profileService.ChangeMyPasswordAsync(User, dto.CurrentPassword, dto.NewPassword);
        return Ok(Result.SuccessResult(Messages.User.PasswordChanged));
    }
}
