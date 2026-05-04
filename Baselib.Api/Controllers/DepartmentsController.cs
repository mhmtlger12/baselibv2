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
        var departments = await _departmentService.GetAllAsync();
        return Ok(DataResult<IEnumerable<DepartmentDto>>.SuccessDataResult(departments));
    }

    [HttpGet("tree")]
    public async Task<IActionResult> Tree()
    {
        var departments = await _departmentService.GetTreeAsync();
        return Ok(DataResult<IEnumerable<DepartmentDto>>.SuccessDataResult(departments));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        return department == null
            ? NotFound(Result.ErrorResult(Messages.Department.NotFound, 404))
            : Ok(DataResult<DepartmentDto>.SuccessDataResult(department));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateDepartmentDto dto)
    {
        var department = await _departmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = department.Id },
            DataResult<DepartmentDto>.SuccessDataResult(department, Messages.General.Saved));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto)
    {
        await _departmentService.UpdateAsync(id, dto);
        return Ok(Result.SuccessResult(Messages.General.Updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _departmentService.DeleteAsync(id);
        return Ok(Result.SuccessResult(Messages.General.Deleted));
    }
}
