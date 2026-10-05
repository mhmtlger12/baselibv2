using Baselib.Api.Attributes;
using Baselib.Business.Content;
using Baselib.Business.DTOs;
using Baselib.Core.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Baselib.Api.Controllers;

[ApiController, Route("api/content"), Authorize(Policy = "DynamicPermission")]
public sealed class ContentController(IEnumerable<IContentModule> modules) : ControllerBase
{
    private IContentModule? Find(string slug) => modules.FirstOrDefault(x => x.Slug == slug);

    [HttpGet("{slug}"), RequirePermission("Content_Read")]
    public async Task<IActionResult> Read(string slug, CancellationToken ct)
    {
        var module = Find(slug);
        return module is null ? NotFound(Result.NotFound()) : Ok(DataResult<ContentModuleDto>.Ok(await module.ReadAsync(ct)));
    }
    [HttpPost("{slug}"), RequirePermission("Content_Create")]
    public async Task<IActionResult> Create(string slug, SaveContentDto input, CancellationToken ct) => await Save(slug, 0, input, ct);
    [HttpPut("{slug}/{id:int:min(1)}"), RequirePermission("Content_Update")]
    public async Task<IActionResult> Update(string slug, int id, SaveContentDto input, CancellationToken ct) => await Save(slug, id, input, ct);
    [HttpDelete("{slug}/{id:int:min(1)}"), RequirePermission("Content_Delete")]
    public async Task<IActionResult> Delete(string slug, int id, CancellationToken ct)
    {
        var module = Find(slug);
        if (module is null) return NotFound(Result.NotFound());
        var result = await module.DeleteAsync(id, ct);
        return StatusCode(result.StatusCode, result);
    }
    private async Task<IActionResult> Save(string slug, int id, SaveContentDto input, CancellationToken ct)
    {
        var module = Find(slug);
        if (module is null) return NotFound(Result.NotFound());
        var result = await module.SaveAsync(id, input, ct);
        return StatusCode(result.StatusCode, result);
    }
}
