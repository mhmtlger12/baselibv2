using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Baselib.Presentation.Models;

namespace Baselib.Presentation.Services;

public class ApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<HttpClient> GetClient()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var client = await GetClient();
        var response = await client.GetAsync(url);
        return await ReadResponseAsync<T>(response);
    }

    public async Task<T?> PostAsync<T>(string url, object? data = null)
    {
        var client = await GetClient();
        var response = await client.PostAsJsonAsync(url, data);
        return await ReadResponseAsync<T>(response);
    }

    public async Task<T?> PutAsync<T>(string url, object? data = null)
    {
        var client = await GetClient();
        var response = await client.PutAsJsonAsync(url, data);
        return await ReadResponseAsync<T>(response);
    }

    public async Task<T?> DeleteAsync<T>(string url)
    {
        var client = await GetClient();
        var response = await client.DeleteAsync(url);
        return await ReadResponseAsync<T>(response);
    }

    private async Task<T?> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            ClearAuthCookies();
            throw new UnauthorizedAccessException("Oturum süresi doldu, lütfen tekrar giriş yapın.");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new InvalidOperationException("Bu içeriği görüntülemek veya bu işlemi yapmak için yetkiniz bulunmuyor.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            if (response.IsSuccessStatusCode)
                return default;

            throw new HttpRequestException($"API isteği başarısız: {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        ApiResponse<T>? result;
        try
        {
            result = JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("API beklenen JSON formatında cevap dönmedi.", ex);
        }
        
        if (result == null)
            return default;

        if (!response.IsSuccessStatusCode || !result.Success)
            throw new InvalidOperationException(result.Message);
        
        return result.Data;
    }

    private void ClearAuthCookies()
    {
        var response = _httpContextAccessor.HttpContext?.Response;
        response?.Cookies.Delete("AccessToken");
        response?.Cookies.Delete("RefreshToken");
        response?.Cookies.Delete("UserData");
    }
}
