using Baselib.Api.Attributes;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DynamicPermission")]
public sealed class SlidersController(ISliderService sliders) : ControllerBase
{
    [HttpGet("published"), AllowAnonymous]
    public async Task<IActionResult> Published(CancellationToken cancellationToken)
    {
        var result = await sliders.GetPublishedAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet, RequirePermission("Sliders_Read")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await sliders.GetAllAsync(cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}"), RequirePermission("Sliders_Read")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var result = await sliders.GetByIdAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost, RequirePermission("Sliders_Create")]
    public async Task<IActionResult> Add(SaveSliderDto dto, CancellationToken cancellationToken = default)
    {
        var result = await sliders.CreateAsync(dto, cancellationToken: cancellationToken);
        if (result.Success) return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}"), RequirePermission("Sliders_Update")]
    public async Task<IActionResult> Update(int id, SaveSliderDto dto, CancellationToken cancellationToken = default)
    {
        var result = await sliders.UpdateAsync(id, dto, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}"), RequirePermission("Sliders_Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var result = await sliders.DeleteAsync(id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
