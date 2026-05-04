using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto);    
        return Ok(DataResult<AuthResultDto>.SuccessDataResult(result));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _userService.RefreshTokenAsync(dto.RefreshToken);
        return Ok(DataResult<AuthResultDto>.SuccessDataResult(result));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        return Ok(DataResult<UserDto>.SuccessDataResult(user, Messages.General.Saved));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _userService.LogoutAsync(User);
        return Ok(Result.SuccessResult(Messages.Auth.LoggedOut));
    }

    [HttpPost("switch-role/{roleId:int}")]
    [Authorize]
    public async Task<IActionResult> SwitchRole(int roleId)
    {
        var result = await _userService.SwitchRoleAsync(User, roleId);
        return Ok(DataResult<AuthResultDto>.SuccessDataResult(result));
    }
}
