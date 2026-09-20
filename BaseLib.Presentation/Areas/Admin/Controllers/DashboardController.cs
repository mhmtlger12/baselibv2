using Baselib.Business.DTOs;
using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class DashboardController(ISystemApiService system) : AdminController
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var stats = await system.DashboardAsync(ct);
        List<AuditLogDto> logs = [];
        string? error = null;
        try { logs = await system.AuditLogsAsync(ct); }
        catch (ApiException ex) when (ex.StatusCode != 401) { error = ex.Message; }
        return View(new DashboardModel(stats, logs.Take(5).ToList(), error));
    }
}
