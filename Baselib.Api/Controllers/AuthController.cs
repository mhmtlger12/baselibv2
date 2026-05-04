using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto);
        return Ok(DataResult<AuthResultDto>.SuccessDataResult(result));
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var result = await _userService.RefreshTokenAsync(refreshToken);
        return Ok(DataResult<AuthResultDto>.SuccessDataResult(result));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst("nameid")!.Value);
        await _userService.LogoutAsync(userId);
        return Ok(Result.SuccessResult(Messages.Auth.LoggedOut));
    }
}