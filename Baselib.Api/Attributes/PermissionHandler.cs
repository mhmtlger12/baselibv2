using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Baselib.Data;
using Baselib.Entities;

namespace Baselib.Api.Attributes;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;

        if (httpContext == null)
        {
            context.Succeed(requirement);
            return;
        }

        var routeData = httpContext.Request.RouteValues;
        var controller = routeData["controller"]?.ToString();
        var action = routeData["action"]?.ToString();

        if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
        {
            context.Succeed(requirement);
            return;
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            context.Fail();
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var userRoleIds = await dbContext.UserRoles
            .Where(ur => ur.UserId == int.Parse(userIdClaim))
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!userRoleIds.Any())
        {
            context.Fail();
            return;
        }

        var permission = await dbContext.Permissions
            .FirstOrDefaultAsync(p =>
                p.ControllerName.ToUpper() == controller.ToUpper() &&
                p.ActionName.ToUpper() == action.ToUpper() &&
                p.IsActive);

        if (permission == null)
        {
            context.Succeed(requirement);
            return;
        }

        var hasAccess = await dbContext.RolePermissions
            .AnyAsync(rp =>
                userRoleIds.Contains(rp.RoleId) &&
                rp.PermissionId == permission.Id);

        if (hasAccess)
        {
            context.Succeed(requirement);
        }
    }
}