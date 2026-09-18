using Baselib.Business.DTOs;
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
    public async Task<IActionResult> List()
    {
        var result = await _menuService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyMenus()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _menuService.GetMenusForUserAsync(userId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Read")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _menuService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Create")]
    public async Task<IActionResult> Add([FromBody] CreateMenuDto dto)
    {
        var result = await _menuService.CreateAsync(dto);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuDto dto)
    {
        var result = await _menuService.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "DynamicPermission")]
    [RequirePermission("Menus_Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _menuService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
