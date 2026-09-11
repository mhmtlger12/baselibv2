using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.Interfaces;

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
    public async Task<IActionResult> List()
    {
        var result = await _recycleBinService.GetAllDeletedItemsAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{type}/{id:int}/restore")]
    public async Task<IActionResult> Restore(string type, int id)
    {
        var result = await _recycleBinService.RestoreAsync(type, id);
        return StatusCode(result.StatusCode, result);
    }
}
