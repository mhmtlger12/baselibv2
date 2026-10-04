using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Api.Attributes;
using Baselib.Core.Constants;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DynamicPermission")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [RequirePermission("Users_Read")]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAllAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("Users_Read")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [RequirePermission("Users_Create")]
    public async Task<IActionResult> Add([FromBody] CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _userService.CreateAsync(dto, User, cancellationToken: cancellationToken);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("Users_Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateAsync(id, dto, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}/password")]
    [RequirePermission(Constants.Permissions.UsersResetPassword)]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetUserPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _userService.ResetPasswordAsync(User, id, dto, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("Users_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.DeleteAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}/roles")]
    [RequirePermission("Users_AssignRoles")]
    public async Task<IActionResult> AssignRoles(int id, [FromBody] List<int> roleIds, CancellationToken cancellationToken = default)
    {
        var result = await _userService.AssignRolesAsync(User, id, roleIds, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
