using System.Net.Http.Json;
using System.Text.Json;

namespace Baselib.Presentation.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private void SetAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<object> GetAsync(string url)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.GetAsync(url);
        return await ReadResponseAsync(response);
    }

    public async Task<object> PostAsync(string url, object? data = null)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync(url, data);
        return await ReadResponseAsync(response);
    }

    public async Task<object> PutAsync(string url, object? data = null)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PutAsJsonAsync(url, data);
        return await ReadResponseAsync(response);
    }

    public async Task<object> DeleteAsync(string url)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.DeleteAsync(url);
        return await ReadResponseAsync(response);
    }

    private async Task<object> ReadResponseAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, object>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        return result ?? new Dictionary<string, object>();
    }
}