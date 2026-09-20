using System.Net.Http.Headers;
using System.Text.Json;
namespace BaseLib.Presentation.Services.Api;

// The only HTTP transport. API envelopes and validation responses are normalized here.
public sealed class ApiTransport(IHttpClientFactory factory)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    public async Task<T> SendAsync<T>(HttpMethod method, string path, object? body = null,
        string? accessToken = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, path);
        if (body is not null) request.Content = JsonContent.Create(body, options: Json);
        if (accessToken is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage response;
        try { response = await factory.CreateClient("BaselibApi").SendAsync(request, cancellationToken); }
        catch (HttpRequestException) { throw new ApiException(503, "API sunucusuna ulaşılamadı. Lütfen tekrar deneyin."); }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        { throw new ApiException(504, "API yanıt süresi aşıldı. Lütfen tekrar deneyin."); }
        using (response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return default!;
            JsonDocument document;
            try { document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken)); }
            catch (JsonException) { throw new ApiException((int)response.StatusCode >= 400 ? (int)response.StatusCode : 502, "API geçerli bir yanıt döndürmedi."); }
            using (document)
            {
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                    throw new ApiException(502, "API beklenen yanıt biçimini döndürmedi.");
                if (!response.IsSuccessStatusCode || (root.TryGetProperty("success", out var success) && success.ValueKind == JsonValueKind.False))
                {
                    var message = root.TryGetProperty("message", out var msg) ? msg.GetString() : "İşlem tamamlanamadı.";
                    var errors = root.TryGetProperty("errors", out var err) ? err.Deserialize<Dictionary<string, string[]>>(Json) : null;
                    throw new ApiException((int)response.StatusCode, message ?? "İşlem tamamlanamadı.", errors);
                }
                if (typeof(T) == typeof(object)) return default!;
                var data = root.TryGetProperty("data", out var value) ? value : root;
                return data.Deserialize<T>(Json) ?? throw new ApiException(502, "API yanıtında beklenen veri bulunamadı.");
            }
        }
    }
}
