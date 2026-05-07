using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Claims;
using Baselib.Business.Interfaces;

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

        var activeRoleIdClaim = context.User.FindFirst("ActiveRoleId")?.Value;
        int? activeRoleId = int.TryParse(activeRoleIdClaim, out var roleId) ? roleId : null;

        using var scope = _serviceProvider.CreateScope();
        var permissionCheckService = scope.ServiceProvider.GetRequiredService<IPermissionCheckService>();

        var hasAccess = await permissionCheckService.HasAccessAsync(userId, activeRoleId, controller, action);

        if (hasAccess)
        {
            context.Succeed(requirement);
        }
    }
}
