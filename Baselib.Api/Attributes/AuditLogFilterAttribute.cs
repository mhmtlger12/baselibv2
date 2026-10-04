using Baselib.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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

        if (resultContext.Exception != null || resultContext.Canceled)
            return;

        // Action sonucu henüz HTTP yanıtına uygulanmadığı için durum kodunu sonuçtan oku.
        var statusCode = resultContext.Result switch
        {
            ObjectResult { Value: ProblemDetails problem } objectResult =>
                objectResult.StatusCode ?? problem.Status ?? context.HttpContext.Response.StatusCode,
            IStatusCodeActionResult statusResult =>
                statusResult.StatusCode ?? context.HttpContext.Response.StatusCode,
            _ => (int?)null
        };
        if (statusCode is not (>= 200 and < 300))
            return;

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
                details = JsonSerializer.Serialize(context.ActionArguments
                    .Where(argument => argument.Value is not CancellationToken)
                    .ToDictionary(argument => argument.Key, argument => argument.Value));
                details = Regex.Replace(details, @"(""(?i)(?:password|currentpassword|newpassword|token|refreshtoken)""\s*:\s*"")[^""]*("")", "$1***$2");
            }
        }
        catch
        {
            details = "Argümanlar serileştirilemedi.";
        }

        // Ayrı scope, audit kaydının istekteki bekleyen entity değişikliklerini kaydetmesini önler.
        await using var auditScope = context.HttpContext.RequestServices.CreateAsyncScope();
        var auditService = auditScope.ServiceProvider.GetRequiredService<IAuditLogService>();
        // A completed mutation still needs its audit record if the client disconnects.
        await auditService.LogAsync(userId, method, controller, route, details, CancellationToken.None);
    }
}
