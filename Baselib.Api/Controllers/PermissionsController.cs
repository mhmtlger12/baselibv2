using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Api.Attributes;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DynamicPermission")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    [RequirePermission("Permissions_Read")]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.GetAllAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("grouped")]
    [RequirePermission("Permissions_Read")]
    public async Task<IActionResult> GroupedList([FromQuery] int? roleId = null, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.GetGroupedPermissionsAsync(roleId, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("Permissions_Read")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.GetByIdAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [RequirePermission("Permissions_Create")]
    public async Task<IActionResult> Add([FromBody] CreatePermissionDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.CreateAsync(dto, cancellationToken: cancellationToken);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("Permissions_Update")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePermissionDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.UpdateAsync(id, dto, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("Permissions_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _permissionService.DeleteAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
