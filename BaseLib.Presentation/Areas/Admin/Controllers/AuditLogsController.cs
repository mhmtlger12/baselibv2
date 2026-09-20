using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class AuditLogsController(ISystemApiService system) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(await system.AuditLogsAsync(ct));
}
