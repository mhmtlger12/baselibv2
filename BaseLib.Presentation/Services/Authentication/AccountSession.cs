using System.Security.Claims;
using Baselib.Business.DTOs;
using BaseLib.Presentation.Services.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace BaseLib.Presentation.Services.Authentication;

public interface IAccountSession
{
    Task LoginAsync(LoginDto input, CancellationToken ct);
    Task LogoutAsync(CancellationToken ct);
    Task SwitchRoleAsync(int roleId, CancellationToken ct);
    Task<AuthResultDto?> GetAsync(CancellationToken ct, string? rejectedToken = null);
}
public sealed class AccountSession(ApiTransport transport, IServerSessionStore store, IHttpContextAccessor accessor) : IAccountSession
{
    public const string SessionClaim = "baselib.session";
    private HttpContext Context => accessor.HttpContext!;
    public async Task LoginAsync(LoginDto input, CancellationToken ct)
    {
        var auth = await transport.SendAsync<AuthResultDto>(HttpMethod.Post, ApiRoutes.Login, input, cancellationToken: ct);
        store.Remove(Context.User.FindFirstValue(SessionClaim));
        await SignInAsync(store.Create(auth), auth.User);
    }
    public async Task<AuthResultDto?> GetAsync(CancellationToken ct, string? rejectedToken = null)
    {
        var id = Context.User.FindFirstValue(SessionClaim);
        var session = store.Find(id);
        if (session is null) return null;
        await session.Gate.WaitAsync(ct);
        try
        {
            if (store.Find(id) is null) return null;
            var auth = session.Authentication;
            if (auth.ExpiryDate.ToUniversalTime() > DateTime.UtcNow.AddSeconds(45) && auth.AccessToken != rejectedToken) return auth;
            try
            {
                session.Authentication = await transport.SendAsync<AuthResultDto>(HttpMethod.Post, ApiRoutes.Refresh,
                    new RefreshTokenRequestDto { RefreshToken = auth.RefreshToken }, cancellationToken: ct);
                return session.Authentication;
            }
            catch (ApiException ex) when (ex.StatusCode is 400 or 401 or 403)
            { store.Remove(id); return null; }
        }
        finally { session.Gate.Release(); }
    }
    public async Task SwitchRoleAsync(int roleId, CancellationToken ct)
    {
        var id = Context.User.FindFirstValue(SessionClaim);
        _ = await GetAsync(ct) ?? throw new ApiException(401, "Oturumunuz sona erdi.");
        var session = store.Find(id) ?? throw new ApiException(401, "Oturumunuz sona erdi.");
        await session.Gate.WaitAsync(ct);
        try
        {
            var updated = await transport.SendAsync<AuthResultDto>(HttpMethod.Post, ApiRoutes.SwitchRole(roleId), accessToken: session.Authentication.AccessToken, cancellationToken: ct);
            session.Authentication = updated;
            await SignInAsync(id!, updated.User);
        }
        finally { session.Gate.Release(); }
    }
    public async Task LogoutAsync(CancellationToken ct)
    {
        var id = Context.User.FindFirstValue(SessionClaim);
        var session = store.Find(id);
        try
        {
            if (session is not null)
                await transport.SendAsync<object>(HttpMethod.Post, ApiRoutes.Logout, accessToken: session.Authentication.AccessToken, cancellationToken: ct);
        }
        finally { store.Remove(id); await Context.SignOutAsync(); }
    }
    public static ClaimsPrincipal Principal(string id, UserDto user) => new(new ClaimsIdentity(new[]
    {
        new Claim(SessionClaim, id), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username), new Claim("display_name", string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName),
        new Claim(ClaimTypes.Role, user.ActiveRoleName ?? "")
    }, CookieAuthenticationDefaults.AuthenticationScheme));
    private Task SignInAsync(string id, UserDto user) => Context.SignInAsync(Principal(id, user),
        new AuthenticationProperties { IsPersistent = false, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });
}
