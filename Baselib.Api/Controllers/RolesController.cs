using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;

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
        var roles = await _roleService.GetAllAsync();
        return Ok(DataResult<IEnumerable<RoleDto>>.SuccessDataResult(roles));
    }

    [HttpGet("selectOption")]
    public async Task<IActionResult> SelectOption()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(DataResult<IEnumerable<RoleDto>>.SuccessDataResult(roles));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var role = await _roleService.GetByIdAsync(id);
        return role == null
            ? NotFound(Result.ErrorResult(Messages.Role.NotFound, 404))
            : Ok(DataResult<RoleDto>.SuccessDataResult(role));
    }

    [HttpGet("{id:int}/permissions")]
    public async Task<IActionResult> GetPermissions(int id)
    {
        var permissions = await _roleService.GetPermissionsByRoleIdAsync(id);
        return Ok(DataResult<IEnumerable<PermissionGroupDto>>.SuccessDataResult(permissions));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateRoleDto dto)
    {
        var role = await _roleService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = role.Id },
            DataResult<RoleDto>.SuccessDataResult(role, Messages.General.Saved));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto)
    {
        await _roleService.UpdateAsync(id, dto);
        return Ok(Result.SuccessResult(Messages.General.Updated));
    }

    [HttpPut("{id:int}/with-permissions")]
    public async Task<IActionResult> UpdateWithPermissions(int id, [FromBody] RoleWithPermissionsDto dto)
    {
        await _roleService.UpdateWithPermissionsAsync(id, dto.Role, dto.PermissionGroups);
        return Ok(Result.SuccessResult(Messages.General.Updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _roleService.DeleteAsync(id);
        return Ok(Result.SuccessResult(Messages.General.Deleted));
    }

    [HttpPut("{id:int}/permissions")]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] List<int> permissionIds)
    {
        await _roleService.AssignPermissionsAsync(id, permissionIds);
        return Ok(Result.SuccessResult("İzinler başarıyla atandı"));
    }
}

public class RoleWithPermissionsDto
{
    public UpdateRoleDto Role { get; set; } = null!;
    public List<PermissionGroupDto> PermissionGroups { get; set; } = new();
}
