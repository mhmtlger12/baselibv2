using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Baselib.Api.Extensions;

public static class RateLimitPolicies
{
    public const string AuthLogin = "auth-login";
    public const string AuthRegister = "auth-register";
    public const string AuthRefresh = "auth-refresh";
}

public static class RateLimitingServiceExtensions
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(RateLimitPolicies.AuthLogin, context => CreateLimiter(
                context,
                permitLimit: 5,
                window: TimeSpan.FromMinutes(1)));
            options.AddPolicy(RateLimitPolicies.AuthRegister, context => CreateLimiter(
                context,
                permitLimit: 3,
                window: TimeSpan.FromHours(1)));
            options.AddPolicy(RateLimitPolicies.AuthRefresh, context => CreateLimiter(
                context,
                permitLimit: 30,
                window: TimeSpan.FromMinutes(1)));
        });

        return services;
    }

    private static RateLimitPartition<string> CreateLimiter(
        HttpContext context,
        int permitLimit,
        TimeSpan window)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueLimit = 0,
                AutoReplenishment = true
            });
    }
}
