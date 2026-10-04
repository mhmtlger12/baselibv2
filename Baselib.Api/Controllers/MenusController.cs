using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Api.Attributes;
using System.Security.Claims;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Read")]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var result = await _menuService.GetAllAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyMenus(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var activeRoleId = ClaimsPrincipalHelper.GetActiveRoleId(User);
        var result = await _menuService.GetMenusForUserAsync(userId, activeRoleId, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Read")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var result = await _menuService.GetByIdAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Create")]
    public async Task<IActionResult> Add([FromBody] CreateMenuDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _menuService.CreateAsync(dto, cancellationToken: cancellationToken);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _menuService.UpdateAsync(id, dto, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _menuService.DeleteAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
