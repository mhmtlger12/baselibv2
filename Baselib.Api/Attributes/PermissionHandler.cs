using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Baselib.Business.Interfaces;

namespace Baselib.Api.Attributes;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionCheckService _permissionCheckService;

    public PermissionHandler(IPermissionCheckService permissionCheckService)
    {
        _permissionCheckService = permissionCheckService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        var httpContext = context.Resource switch
        {
            HttpContext currentHttpContext => currentHttpContext,
            AuthorizationFilterContext authorizationFilterContext => authorizationFilterContext.HttpContext,
            _ => null
        };
        var permission = httpContext?.GetEndpoint()?.Metadata.GetMetadata<RequirePermissionAttribute>();

        // Eksik/çözülemeyen metadata bir yapılandırma hatasıdır; asla erişim izni değildir.
        if (httpContext == null || permission == null || string.IsNullOrWhiteSpace(permission.Code))
        {
            context.Fail();
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

        var hasAccess = await _permissionCheckService.HasAccessAsync(userId, activeRoleId, permission.Code);

        if (hasAccess)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
