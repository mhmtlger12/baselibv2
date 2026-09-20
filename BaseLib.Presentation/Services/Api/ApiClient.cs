using BaseLib.Presentation.Services.Authentication;
namespace BaseLib.Presentation.Services.Api;
public sealed class ApiClient(ApiTransport transport, IAccountSession sessions) : IApiClient
{
    public Task<T> GetAsync<T>(string path, CancellationToken ct = default) => SendAsync<T>(HttpMethod.Get, path, null, ct);
    public async Task SendAsync(HttpMethod method, string path, object? body = null, CancellationToken ct = default)
        => await SendAsync<object>(method, path, body, ct);
    private async Task<T> SendAsync<T>(HttpMethod method, string path, object? body, CancellationToken ct)
    {
        var auth = await sessions.GetAsync(ct) ?? throw new ApiException(401, "Oturumunuz sona erdi. Lütfen tekrar giriş yapın.");
        try { return await transport.SendAsync<T>(method, path, body, auth.AccessToken, ct); }
        catch (ApiException ex) when (ex.StatusCode == 401)
        {
            auth = await sessions.GetAsync(ct, auth.AccessToken) ?? throw new ApiException(401, "Oturumunuz sona erdi. Lütfen tekrar giriş yapın.");
            return await transport.SendAsync<T>(method, path, body, auth.AccessToken, ct);
        }
    }
}
