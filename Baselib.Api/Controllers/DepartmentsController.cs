using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Api.Attributes;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DynamicPermission")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [RequirePermission("Departments_Read")]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.GetAllAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("selectOption")]
    [RequirePermission("Departments_SelectOption")]
    public async Task<IActionResult> SelectOption(CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.GetSelectOptionsAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("tree")]
    [RequirePermission("Departments_Read")]
    public async Task<IActionResult> Tree(CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.GetTreeAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("Departments_Read")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.GetByIdAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [RequirePermission("Departments_Create")]
    public async Task<IActionResult> Add([FromBody] CreateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.CreateAsync(dto, cancellationToken: cancellationToken);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("Departments_Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.UpdateAsync(id, dto, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("Departments_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await _departmentService.DeleteAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
