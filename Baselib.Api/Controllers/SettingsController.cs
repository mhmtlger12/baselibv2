using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Api.Attributes;

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
    [RequirePermission("Settings_Read")]
    public async Task<IActionResult> List()
    {
        var result = await _settingService.GetAllAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id:int}")]
    [RequirePermission("Settings_Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSettingDto dto)
    {
        var result = await _settingService.UpdateAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }
}
