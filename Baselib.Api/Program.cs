using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Baselib.Api.Attributes;
using Baselib.Api.Extensions;
using Baselib.Api.Middleware;
using Baselib.Business.Extensions;
using Baselib.Core.Constants;
using Baselib.Core.Interfaces;
using Baselib.Data.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 64 * 1024);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<Baselib.Api.Attributes.AuditLogFilterAttribute>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddApiRateLimiting();
builder.Services.AddCors(options =>
{
    options.AddPolicy("PresentationClient", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5011",
                "https://localhost:7230",
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:8443",
                "http://127.0.0.1:8443")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDataServices(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration[Constants.Jwt.Issuer],
            ValidAudience = builder.Configuration[Constants.Jwt.Audience],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[Constants.Jwt.Key]!))
        };
    });

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DynamicPermission", policy =>
        policy.Requirements.Add(new PermissionRequirement()));
});

builder.Services.AddBusinessServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("PresentationClient");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
