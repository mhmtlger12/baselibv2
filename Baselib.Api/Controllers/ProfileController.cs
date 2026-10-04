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
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken = default)
    {
        var result = await _profileService.GetMyProfileAsync(User, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _profileService.ChangeMyPasswordAsync(User, dto.CurrentPassword, dto.NewPassword, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
