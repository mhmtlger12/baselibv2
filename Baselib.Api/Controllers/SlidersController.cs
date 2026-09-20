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
    public async Task<IActionResult> Published(CancellationToken ct)
    {
        var result = await sliders.GetPublishedAsync(ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet, RequirePermission("Sliders_Read")]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await sliders.GetAllAsync(ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id:int}"), RequirePermission("Sliders_Read")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await sliders.GetByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost, RequirePermission("Sliders_Create")]
    public async Task<IActionResult> Add(SaveSliderDto dto)
    {
        var result = await sliders.CreateAsync(dto);
        if (result.Success) return CreatedAtAction(nameof(Get), new { id = result.Data.Id }, result);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}"), RequirePermission("Sliders_Update")]
    public async Task<IActionResult> Update(int id, SaveSliderDto dto)
    {
        var result = await sliders.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id:int}"), RequirePermission("Sliders_Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await sliders.DeleteAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
