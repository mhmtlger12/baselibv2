using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.Interfaces;
using Baselib.Api.Attributes;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DynamicPermission")]
public class RecycleBinController : ControllerBase
{
    private readonly IRecycleBinService _recycleBinService;

    public RecycleBinController(IRecycleBinService recycleBinService)
    {
        _recycleBinService = recycleBinService;
    }

    [HttpGet]
    [RequirePermission("RecycleBin_Read")]
    public async Task<IActionResult> List(CancellationToken cancellationToken = default)
    {
        var result = await _recycleBinService.GetAllDeletedItemsAsync(cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{type}/{id:int}/restore")]
    [RequirePermission("RecycleBin_Restore")]
    public async Task<IActionResult> Restore(string type, int id, CancellationToken cancellationToken = default)
    {
        var result = await _recycleBinService.RestoreAsync(type, id, cancellationToken: cancellationToken);
        return StatusCode(result.StatusCode, result);
    }
}
