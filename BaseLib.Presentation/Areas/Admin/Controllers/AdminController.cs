using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
[Area("Admin"), Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public abstract class AdminController : Controller
{
    protected async Task<bool> ExecuteAsync(Func<Task> action)
    {
        try { await action(); return true; }
        catch (ApiException error) when (error.StatusCode is not (401 or 403))
        {
            ModelState.AddModelError("", error.Message);
            foreach (var entry in error.Errors)
                foreach (var message in entry.Value) ModelState.AddModelError(entry.Key, message);
            return false;
        }
    }
    protected IActionResult Saved(string message = "Değişiklikler kaydedildi.")
    { TempData["Success"] = message; return RedirectToAction("Index"); }
}
