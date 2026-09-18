using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Api.Attributes;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DynamicPermission")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [RequirePermission("Roles_Read")]
    public async Task<IActionResult> List()
    {
        var result = await _roleService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("selectOption")]
    [RequirePermission("Roles_SelectOption")]
    public async Task<IActionResult> SelectOption()
    {
        var result = await _roleService.GetSelectOptionsAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("Roles_Read")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}/permissions")]
    [RequirePermission("Roles_Read")]
    public async Task<IActionResult> GetPermissions(int id)
    {
        var result = await _roleService.GetPermissionsByRoleIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [RequirePermission("Roles_Create")]
    public async Task<IActionResult> Add([FromBody] CreateRoleDto dto)
    {
        var result = await _roleService.CreateAsync(dto);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("Roles_Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto)
    {
        var result = await _roleService.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }


    [HttpDelete("{id:int}")]
    [RequirePermission("Roles_Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _roleService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}/permissions")]
    [RequirePermission("Roles_Update")]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] List<int> permissionIds)
    {
        var result = await _roleService.AssignPermissionsAsync(id, permissionIds);
        return StatusCode(result.StatusCode, result);
    }
}
