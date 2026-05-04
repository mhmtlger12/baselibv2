using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Results;

namespace Baselib.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Everyone logged in can see dashboard stats
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken token)
    {
        var stats = await _dashboardService.GetStatsAsync(token);
        return Ok(DataResult<DashboardStatsDto>.SuccessDataResult(stats));
    }
}
