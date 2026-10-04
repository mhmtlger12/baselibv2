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
    public async Task<IActionResult> Published([FromQuery] string? categoryKey, [FromQuery] string? q, CancellationToken cancellationToken)
    {
        var result = await jobs.GetPublishedAsync(categoryKey, q, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet, RequirePermission("Jobs_Read")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await jobs.GetAllAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("published/{id:int}"), AllowAnonymous]
    public async Task<IActionResult> PublishedDetail(int id, CancellationToken cancellationToken)
    {
        var result = await jobs.GetPublishedByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}"), RequirePermission("Jobs_Read")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var result = await jobs.GetByIdAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost, RequirePermission("Jobs_Create")]
    public async Task<IActionResult> Add(SaveJobListingDto dto, CancellationToken cancellationToken)
    {
        var result = await jobs.CreateAsync(dto, cancellationToken);
        if (result.Success) return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}"), RequirePermission("Jobs_Update")]
    public async Task<IActionResult> Update(int id, SaveJobListingDto dto, CancellationToken cancellationToken)
    {
        var result = await jobs.UpdateAsync(id, dto, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}"), RequirePermission("Jobs_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await jobs.DeleteAsync(id, cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
