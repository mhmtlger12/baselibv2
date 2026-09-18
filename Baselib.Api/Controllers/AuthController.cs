using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Baselib.Api.Extensions;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;

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
    [EnableRateLimiting(RateLimitPolicies.AuthLogin)]
    [RequestSizeLimit(16 * 1024)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto, GetClientSessionInfo());
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting(RateLimitPolicies.AuthRefresh)]
    [RequestSizeLimit(16 * 1024)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken, GetClientSessionInfo());
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.AuthRegister)]
    [RequestSizeLimit(16 * 1024)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _userService.RegisterAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync(User);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("switch-role/{roleId:int}")]
    [Authorize]
    public async Task<IActionResult> SwitchRole(int roleId)
    {
        var result = await _authService.SwitchRoleAsync(User, roleId, GetClientSessionInfo());
        return StatusCode(result.StatusCode, result);
    }

    private ClientSessionInfoDto GetClientSessionInfo()
    {
        return new ClientSessionInfoDto
        {
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString()
        };
    }
}
