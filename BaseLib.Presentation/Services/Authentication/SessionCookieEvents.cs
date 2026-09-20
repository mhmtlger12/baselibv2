using System.Security.Claims;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace BaseLib.Presentation.Services.Authentication;
public sealed class SessionCookieEvents(IAccountSession sessions) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        // Make the candidate principal available before the authentication middleware completes.
        var originalPrincipal = context.HttpContext.User;
        context.HttpContext.User = context.Principal!;
        try
        {
            var auth = await sessions.GetAsync(context.HttpContext.RequestAborted);
            if (auth is null)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync();
                return;
            }
            var id = context.Principal!.FindFirstValue(AccountSession.SessionClaim)!;
            var principal = AccountSession.Principal(id, auth.User);
            context.ShouldRenew = context.Principal!.FindFirstValue(ClaimTypes.Role) != auth.User.ActiveRoleName;
            context.ReplacePrincipal(principal);
        }
        catch (ApiException)
        {
            // A temporary upstream outage must not delete a valid session.
            // Subsequent API calls display the service error, while cached session identity remains valid.
        }
        finally { context.HttpContext.User = originalPrincipal; }
    }
}
