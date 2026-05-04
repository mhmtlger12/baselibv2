using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Baselib.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

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
        var action = context.RouteData.Values["action"]?.ToString() ?? "Unknown";
        var route = context.HttpContext.Request.Path;
        
        string details = "";
        try
        {
            // Eğer varsa Action argümanlarını JSON olarak kaydet
            if (context.ActionArguments.Any())
            {
                details = JsonSerializer.Serialize(context.ActionArguments);
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
