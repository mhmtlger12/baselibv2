using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class SettingsController(ISystemApiService system) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(await system.SettingsAsync(ct));
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var item = (await system.SettingsAsync(ct)).Find(x => x.Id == id);
        return item is null ? NotFound() : View(new SettingEditModel { Id = id, Key = item.Key, Value = item.Value, Description = item.Description });
    }
    [HttpPost]
    public async Task<IActionResult> Edit(SettingEditModel model, CancellationToken ct)
    {
        if (ModelState.IsValid && await ExecuteAsync(() => system.UpdateSettingAsync(model.Id, model, ct))) return Saved();
        var item = (await system.SettingsAsync(ct)).Find(x => x.Id == model.Id);
        if (item is null) return NotFound();
        model.Key = item.Key; model.Description = item.Description;
        return View(model);
    }
}
