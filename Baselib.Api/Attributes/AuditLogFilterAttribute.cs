using Baselib.Business.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Baselib.Api.Attributes;

public class AuditLogFilterAttribute : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Önce işlemi çalıştır
        var resultContext = await next();

        // Sadece başarılı olan POST, PUT, DELETE isteklerini logla
        var method = context.HttpContext.Request.Method;
        if (method != "POST" && method != "PUT" && method != "DELETE")
            return;

        if (resultContext.Exception != null || context.HttpContext.Response.StatusCode >= 400)
            return; // Hata alan işlemleri loglama (isteğe bağlı)

        var userIdClaim = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? userId = null;
        if (int.TryParse(userIdClaim, out int id))
        {
            userId = id;
        }

        var controller = context.RouteData.Values["controller"]?.ToString() ?? "Unknown";
        var route = context.HttpContext.Request.Path;

        string details = "";
        try
        {
            // Eğer varsa Action argümanlarını JSON olarak kaydet ve hassas alanları maskele
            if (context.ActionArguments.Any())
            {
                details = JsonSerializer.Serialize(context.ActionArguments);
                details = Regex.Replace(details, @"(""(?i)(?:password|currentpassword|newpassword|token|refreshtoken)""\s*:\s*"")[^""]*("")", "$1***$2");
            }
        }
        catch
        {
            details = "Argümanlar serileştirilemedi.";
        }

        // Service Locator pattern ile servisi çekiyoruz (Filter içinde DI için)
        var auditService = context.HttpContext.RequestServices.GetService<IAuditLogService>();
        if (auditService != null)
        {
            await auditService.LogAsync(userId, method, controller, route, details);
        }
    }
}
