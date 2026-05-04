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
public class SettingsController : ControllerBase
{
    private readonly ISettingService _settingService;

    public SettingsController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var settings = await _settingService.GetAllAsync();
        return Ok(DataResult<IEnumerable<SettingDto>>.SuccessDataResult(settings));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSettingDto dto)
    {
        try
        {
            await _settingService.UpdateAsync(id, dto);
            return Ok(Result.SuccessResult(Messages.General.Updated));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(Result.ErrorResult("Ayar bulunamadı.", 404));
        }
    }
}
