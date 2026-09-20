using Baselib.Api.Attributes;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DynamicPermission")]
public sealed class JobsController(IJobListingService jobs) : ControllerBase
{
    [HttpGet("published"), AllowAnonymous]
    public async Task<IActionResult> Published([FromQuery] string? categoryKey, [FromQuery] string? q, CancellationToken ct)
    {
        var result = await jobs.GetPublishedAsync(categoryKey, q, ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet, RequirePermission("Jobs_Read")]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await jobs.GetAllAsync(ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}"), AllowAnonymous]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var result = await jobs.GetByIdAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost, RequirePermission("Jobs_Create")]
    public async Task<IActionResult> Add(SaveJobListingDto dto, CancellationToken ct)
    {
        var result = await jobs.CreateAsync(dto, ct);
        if (result.Success) return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}"), RequirePermission("Jobs_Update")]
    public async Task<IActionResult> Update(int id, SaveJobListingDto dto, CancellationToken ct)
    {
        var result = await jobs.UpdateAsync(id, dto, ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}"), RequirePermission("Jobs_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await jobs.DeleteAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }
}
