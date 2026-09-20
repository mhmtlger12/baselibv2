using Baselib.Business.DTOs;
using BaseLib.Presentation.Services.Api;
using BaseLib.Presentation.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BaseLib.Presentation.Areas.Admin.Controllers;
public sealed class AccountController(IAccountSession session, ILogger<AccountController> logger) : AdminController
{
    [AllowAnonymous, HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
        ViewData["ReturnUrl"] = Url.IsLocalUrl(returnUrl) ? returnUrl : "/Admin";
        return View(new LoginDto());
    }
    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> Login(LoginDto input, string? returnUrl, CancellationToken ct)
    {
        ViewData["ReturnUrl"] = Url.IsLocalUrl(returnUrl) ? returnUrl : "/Admin";
        if (!ModelState.IsValid) return View(input);
        try
        {
            await session.LoginAsync(input, ct);
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/Admin");
        }
        catch (ApiException error) { ModelState.AddModelError("", error.Message); return View(input); }
    }
    [HttpPost]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        try { await session.LogoutAsync(ct); }
        catch (ApiException error) { logger.LogWarning("Upstream logout failed with status {Status}.", error.StatusCode); }
        return RedirectToAction(nameof(Login));
    }
    [HttpGet] public IActionResult AccessDenied() { Response.StatusCode = 403; return View(); }
}
