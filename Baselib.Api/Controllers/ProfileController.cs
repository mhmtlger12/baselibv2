using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Results;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;

    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = await _userService.GetMyProfileAsync(User);
        return Ok(DataResult<UserDto>.SuccessDataResult(user));
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        await _userService.ChangeMyPasswordAsync(User, dto.CurrentPassword, dto.NewPassword);
        return Ok(Result.SuccessResult("Şifreniz başarıyla güncellendi."));
    }
}
