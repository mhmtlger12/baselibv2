using System.Net.Http.Json;
using System.Text.Json;
using Baselib.Presentation.Models;

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

    public async Task<T?> GetAsync<T>(string url)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.GetAsync(url);
        return await ReadResponseAsync<T>(response);
    }

    public async Task<T?> PostAsync<T>(string url, object? data = null)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync(url, data);
        return await ReadResponseAsync<T>(response);
    }

    public async Task<T?> PutAsync<T>(string url, object? data = null)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PutAsJsonAsync(url, data);
        return await ReadResponseAsync<T>(response);
    }

    public async Task<T?> DeleteAsync<T>(string url)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.DeleteAsync(url);
        return await ReadResponseAsync<T>(response);
    }

    private async Task<T?> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (result == null)
            return default;
        
        return result.Data;
    }
}