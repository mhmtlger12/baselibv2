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
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var permissions = await _permissionService.GetAllAsync();
        return Ok(DataResult<IEnumerable<PermissionDto>>.SuccessDataResult(permissions));
    }

    [HttpGet]
    public async Task<IActionResult> GroupedList([FromQuery] int? roleId = null)
    {
        var permissions = await _permissionService.GetGroupedPermissionsAsync(roleId);
        return Ok(DataResult<IEnumerable<PermissionGroupDto>>.SuccessDataResult(permissions));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var permission = await _permissionService.GetByIdAsync(id);
        return permission == null
            ? NotFound(Result.ErrorResult(Messages.Permission.NotFound, 404))
            : Ok(DataResult<PermissionDto>.SuccessDataResult(permission));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreatePermissionDto dto)
    {
        var permission = await _permissionService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = permission.Id },
            DataResult<PermissionDto>.SuccessDataResult(permission, Messages.General.Saved));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePermissionDto dto)
    {
        await _permissionService.UpdateAsync(id, dto);
        return Ok(Result.SuccessResult(Messages.General.Updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _permissionService.DeleteAsync(id);
        return Ok(Result.SuccessResult(Messages.General.Deleted));
    }

    [HttpPost("role/{roleId}/permissions")]
    public async Task<IActionResult> SaveRolePermissions(int roleId, [FromBody] List<PermissionGroupDto> permissionGroups)
    {
        await _permissionService.SaveRolePermissionsAsync(roleId, permissionGroups);
        return Ok(Result.SuccessResult(Messages.General.Updated));
    }
}