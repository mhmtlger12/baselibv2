using Baselib.Api.Attributes;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DynamicPermission")]
public sealed class InstitutionsController(IInstitutionService institutions) : ControllerBase
{
    [HttpGet, RequirePermission("Institutions_Read")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await institutions.GetAllAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}"), RequirePermission("Institutions_Read")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var result = await institutions.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost, RequirePermission("Institutions_Create")]
    public async Task<IActionResult> Add(SaveInstitutionDto dto, CancellationToken cancellationToken)
    {
        var result = await institutions.CreateAsync(dto, cancellationToken);
        if (result.Success) return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}"), RequirePermission("Institutions_Update")]
    public async Task<IActionResult> Update(int id, SaveInstitutionDto dto, CancellationToken cancellationToken)
    {
        var result = await institutions.UpdateAsync(id, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}"), RequirePermission("Institutions_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await institutions.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
