using BaseLib.Presentation.Areas.Admin.Models;
using BaseLib.Presentation.Services.Api;
using BaseLib.Presentation.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class ProfileController(ISystemApiService system, IAccountSession session) : AdminController
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) => View(new ProfileModel { User = await system.ProfileAsync(ct) });
    [HttpPost]
    public async Task<IActionResult> Password([Bind(Prefix = "Password")] PasswordModel input, CancellationToken ct)
    {
        if (ModelState.IsValid && await ExecuteAsync(() => system.ChangePasswordAsync(input, ct))) return Saved("Şifreniz güncellendi.");
        return View("Index", new ProfileModel { User = await system.ProfileAsync(ct), Password = input });
    }
    [HttpPost]
    public async Task<IActionResult> SwitchRole(int roleId, CancellationToken ct)
    {
        if (await ExecuteAsync(() => session.SwitchRoleAsync(roleId, ct))) return RedirectToAction("Index", "Dashboard");
        return View("Index", new ProfileModel { User = await system.ProfileAsync(ct) });
    }
}
