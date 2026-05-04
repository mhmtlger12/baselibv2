using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Baselib.Data;

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
        var actionDescriptor = httpContext.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
        var controller = routeData["controller"]?.ToString() ?? actionDescriptor?.ControllerName;
        var action = routeData["action"]?.ToString() ?? actionDescriptor?.ActionName;

        if (string.IsNullOrWhiteSpace(controller) || string.IsNullOrWhiteSpace(action))
        {
            context.Succeed(requirement);
            return;
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userIdClaim, out var userId))
        {
            context.Fail();
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var activeRoleIdClaim = context.User.FindFirst("ActiveRoleId")?.Value;
        
        // Veritabanından kullanıcının GÜNCEL rollerini çekiyoruz (Güvenlik için şart)
        var currentUserRoleIds = await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!currentUserRoleIds.Any())
        {
            context.Fail();
            return;
        }

        List<int> effectiveRoleIds;

        if (!string.IsNullOrEmpty(activeRoleIdClaim) && int.TryParse(activeRoleIdClaim, out var activeRoleId))
        {
            // JWT içindeki aktif rol veritabanında HALA mevcut mu? (Revoke kontrolü)
            if (currentUserRoleIds.Contains(activeRoleId))
            {
                effectiveRoleIds = new List<int> { activeRoleId };
            }
            else
            {
                // Rol geri alınmış ama token süresi bitmemiş. Erişimi reddet.
                context.Fail();
                return;
            }
        }
        else
        {
            effectiveRoleIds = currentUserRoleIds;
        }

        if (!effectiveRoleIds.Any())
        {
            context.Fail();
            return;
        }

        var permissionIds = await dbContext.Permissions
            .Where(p =>
                p.ControllerName.ToUpper() == controller.ToUpper() &&
                p.ActionName.ToUpper() == action.ToUpper() &&
                p.IsActive)
            .Select(p => p.Id)
            .ToListAsync();

        if (!permissionIds.Any())
        {
            context.Succeed(requirement);
            return;
        }

        var hasAccess = await dbContext.RolePermissions
            .AnyAsync(rp =>
                effectiveRoleIds.Contains(rp.RoleId) &&
                permissionIds.Contains(rp.PermissionId));

        if (hasAccess)
        {
            context.Succeed(requirement);
        }
    }
}
