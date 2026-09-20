using BaseLib.Presentation.Services.Api;
using BaseLib.Presentation.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
namespace BaseLib.Presentation.Filters;
public sealed class ApiExceptionFilter(IServerSessionStore sessions) : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is not ApiException error) return;
        context.ExceptionHandled = true;
        if (error.StatusCode == 401)
        {
            sessions.Remove(context.HttpContext.User.FindFirstValue(AccountSession.SessionClaim));
            await context.HttpContext.SignOutAsync();
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin", returnUrl = context.HttpContext.Request.Path.Value });
            return;
        }
        context.Result = new ViewResult
        {
            ViewName = "~/Views/Shared/Error.cshtml",
            StatusCode = error.StatusCode >= 400 ? error.StatusCode : 502,
            ViewData = new ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(), context.ModelState)
            { ["Title"] = "İşlem tamamlanamadı", ["Message"] = error.Message }
        };
    }
}
