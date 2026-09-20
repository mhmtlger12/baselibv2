using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class RecycleBinController(ISystemApiService system) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(await system.RecycleBinAsync(ct));
    [HttpPost]
    public async Task<IActionResult> Restore(string type, int id, CancellationToken ct)
    {
        if (await ExecuteAsync(() => system.RestoreAsync(type, id, ct))) return Saved("Kayıt geri yüklendi.");
        return View("Index", await system.RecycleBinAsync(ct));
    }
}
