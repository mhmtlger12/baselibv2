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
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);    
        return Ok(DataResult<AuthResultDto>.SuccessDataResult(result));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
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
        await _authService.LogoutAsync(User);
        return Ok(Result.SuccessResult(Messages.Auth.LoggedOut));
    }

    [HttpPost("switch-role/{roleId:int}")]
    [Authorize]
    public async Task<IActionResult> SwitchRole(int roleId)
    {
        var result = await _authService.SwitchRoleAsync(User, roleId);
        return Ok(DataResult<AuthResultDto>.SuccessDataResult(result));
    }
}
