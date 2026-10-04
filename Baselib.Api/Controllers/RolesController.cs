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
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var result = await _roleService.GetAllAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("selectOption")]
    [RequirePermission("Roles_SelectOption")]
    public async Task<IActionResult> SelectOption(CancellationToken cancellationToken = default)
    {
        var result = await _roleService.GetSelectOptionsAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("Roles_Read")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var result = await _roleService.GetByIdAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}/permissions")]
    [RequirePermission("Roles_Read")]
    public async Task<IActionResult> GetPermissions(int id, CancellationToken cancellationToken = default)
    {
        var result = await _roleService.GetPermissionsByRoleIdAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [RequirePermission("Roles_Create")]
    public async Task<IActionResult> Add([FromBody] CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _roleService.CreateAsync(User, dto, cancellationToken: cancellationToken);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("Roles_Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _roleService.UpdateAsync(User, id, dto, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }


    [HttpDelete("{id:int}")]
    [RequirePermission("Roles_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _roleService.DeleteAsync(User, id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}/permissions")]
    [RequirePermission("Roles_Update")]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] List<int> permissionIds, CancellationToken cancellationToken = default)
    {
        var result = await _roleService.AssignPermissionsAsync(User, id, permissionIds, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
