using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;

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
    public async Task<IActionResult> List()
    {
        var result = await _departmentService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("selectOption")]
    public async Task<IActionResult> SelectOption()
    {
        var result = await _departmentService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> Tree()
    {
        var result = await _departmentService.GetTreeAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _departmentService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateDepartmentDto dto)
    {
        var result = await _departmentService.CreateAsync(dto);
        if (result.Success && result.Data != null)
            return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);

        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto)
    {
        var result = await _departmentService.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _departmentService.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
