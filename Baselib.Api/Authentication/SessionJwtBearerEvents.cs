using System.Security.Claims;
using Baselib.Business.Interfaces;
using Baselib.Core.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Baselib.Api.Authentication;

public sealed class SessionJwtBearerEvents(ISessionService sessions) : JwtBearerEvents
{
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var principal = context.Principal;
        var familyId = principal?.FindFirstValue(Constants.Jwt.SessionIdClaim);
        var roleClaim = principal?.FindFirstValue("ActiveRoleId");
        int? roleId = int.TryParse(roleClaim, out var parsedRole) ? parsedRole : null;
        if (!int.TryParse(principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
            string.IsNullOrWhiteSpace(familyId) || (roleClaim is not null && roleId is null) ||
            !await sessions.IsActiveAsync(userId, familyId, roleId, context.HttpContext.RequestAborted))
            context.Fail("Oturum geçerli değil.");
    }
}
