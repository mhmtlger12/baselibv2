using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Policy = "DynamicPermission")]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var menus = await _menuService.GetAllAsync();
        return Ok(DataResult<IEnumerable<MenuDto>>.SuccessDataResult(menus));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var menus = await _menuService.GetMenusByUserIdAsync(userId);
        return Ok(DataResult<IEnumerable<MenuDto>>.SuccessDataResult(menus));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var menu = await _menuService.GetByIdAsync(id);
        return menu == null
            ? NotFound(Result.ErrorResult(Messages.Menu.NotFound, 404))
            : Ok(DataResult<MenuDto>.SuccessDataResult(menu));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateMenuDto dto)
    {
        var menu = await _menuService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = menu.Id },
            DataResult<MenuDto>.SuccessDataResult(menu, Messages.General.Saved));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuDto dto)
    {
        await _menuService.UpdateAsync(id, dto);
        return Ok(Result.SuccessResult(Messages.General.Updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _menuService.DeleteAsync(id);
        return Ok(Result.SuccessResult(Messages.General.Deleted));
    }
}