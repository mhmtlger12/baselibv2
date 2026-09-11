using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;

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
        var result = await _profileService.GetMyProfileAsync(User);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _profileService.ChangeMyPasswordAsync(User, dto.CurrentPassword, dto.NewPassword);
        return StatusCode(result.StatusCode, result);
    }
}
