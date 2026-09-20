namespace BaseLib.Presentation.Services.Api;
public interface IApiClient
{
    Task<T> GetAsync<T>(string path, CancellationToken ct = default);
    Task SendAsync(HttpMethod method, string path, object? body = null, CancellationToken ct = default);
}
