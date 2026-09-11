using Baselib.Core.Results;
using System.Text.Json;

namespace Baselib.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            KeyNotFoundException => Result.ErrorResult("Kayıt bulunamadı", 404),
            UnauthorizedAccessException => Result.ErrorResult("Yetkisiz erişim", 401),
            InvalidOperationException => Result.ErrorResult(exception.Message, 400),
            _ => Result.ErrorResult("Bir hata oluştu", 500)
        };

        context.Response.StatusCode = response.StatusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
