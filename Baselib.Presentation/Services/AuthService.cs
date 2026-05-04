using Baselib.Business.DTOs;
using Baselib.Presentation.Models;
using System.Text.Json;

namespace Baselib.Presentation.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TokenCookieName = "AccessToken";
    private const string RefreshTokenCookieName = "RefreshToken";
    private const string UserCookieName = "UserData";

    public AuthService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResultDto?> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/Auth/Login", new { username, password });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResultDto>>();
            if (result?.Data != null)
            {
                var token = result.Data.AccessToken;
                var user = result.Data.User;

                _httpContextAccessor.HttpContext?.Response.Cookies.Append(TokenCookieName, token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddMinutes(15)
                });

                _httpContextAccessor.HttpContext?.Response.Cookies.Append(RefreshTokenCookieName, result.Data.RefreshToken ?? "", new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                var userJson = JsonSerializer.Serialize(user);
                _httpContextAccessor.HttpContext?.Response.Cookies.Append(UserCookieName, userJson, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7)
                });

                return result.Data;
            }
        }
        return null;
    }

    public async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var response = await _httpClient.PostAsJsonAsync("/api/Auth/Refresh", new { refreshToken });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResultDto>>();
            if (result?.Data != null)
            {
                _httpContextAccessor.HttpContext?.Response.Cookies.Append(TokenCookieName, result.Data.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddMinutes(15)
                });
                return true;
            }
        }
        return false;
    }

    public void Logout()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(TokenCookieName);
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(RefreshTokenCookieName);
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(UserCookieName);
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[TokenCookieName] != null;
    }

    public UserDto? GetCurrentUser()
    {
        var userJson = _httpContextAccessor.HttpContext?.Request.Cookies[UserCookieName];
        if (string.IsNullOrEmpty(userJson))
            return null;

        return JsonSerializer.Deserialize<UserDto>(userJson);
    }

    public string? GetToken()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[TokenCookieName];
    }
}