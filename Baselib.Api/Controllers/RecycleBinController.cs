using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Results;

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
        var items = await _recycleBinService.GetAllDeletedItemsAsync();
        return Ok(DataResult<IEnumerable<RecycleBinItemDto>>.SuccessDataResult(items));
    }

    [HttpPut("{type}/{id:int}/restore")]
    public async Task<IActionResult> Restore(string type, int id)
    {
        try
        {
            await _recycleBinService.RestoreAsync(type, id);
            return Ok(Result.SuccessResult("Kayıt başarıyla geri yüklendi."));
        }
        catch (Exception ex)
        {
            return BadRequest(Result.ErrorResult(ex.Message));
        }
    }
}
