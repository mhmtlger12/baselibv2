using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;

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
    public async Task<IActionResult> List()
    {
        var result = await _roleService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("selectOption")]
    public async Task<IActionResult> SelectOption()
    {
        var result = await _roleService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}/permissions")]
    public async Task<IActionResult> GetPermissions(int id)
    {
        var result = await _roleService.GetPermissionsByRoleIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateRoleDto dto)
    {
        var result = await _roleService.CreateAsync(dto);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto)
    {
        var result = await _roleService.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}/with-permissions")]
    public async Task<IActionResult> UpdateWithPermissions(int id, [FromBody] RoleWithPermissionsDto dto)
    {
        var result = await _roleService.UpdateWithPermissionsAsync(id, dto.Role, dto.PermissionGroups);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _roleService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}/permissions")]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] List<int> permissionIds)
    {
        var result = await _roleService.AssignPermissionsAsync(id, permissionIds);
        return StatusCode(result.StatusCode, result);
    }
}
