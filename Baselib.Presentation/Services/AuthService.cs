using Baselib.Presentation.Models;

namespace Baselib.Presentation.Services;

public class AuthService
{
    private readonly HttpContext _httpContext;
    private const string TokenCookieName = "AccessToken";
    private const string RefreshTokenCookieName = "RefreshToken";
    private const string UserCookieName = "UserData";

    public AuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<LoginResult?> LoginAsync(string username, string password)
    {
        using var client = new HttpClient();
        var response = await client.PostAsJsonAsync("https://localhost:7001/api/Auth/Login", new 
        { 
            username, 
            password 
        });

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(content, 
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result?.Data == null)
            return null;

        var loginResult = new LoginResult
        {
            UserId = result.Data.User.Id,
            Username = result.Data.User.Username,
            Email = result.Data.User.Email,
            Roles = result.Data.User.Roles
        };

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.Now.AddDays(7)
        };

        _httpContext.Response.Cookies.Append(TokenCookieName, result.Data.AccessToken, cookieOptions);
        _httpContext.Response.Cookies.Append(RefreshTokenCookieName, result.Data.RefreshToken, cookieOptions);

        return loginResult;
    }

    public void Logout()
    {
        _httpContext.Response.Cookies.Delete(TokenCookieName);
        _httpContext.Response.Cookies.Delete(RefreshTokenCookieName);
    }

    public bool IsAuthenticated => _httpContext.Request.Cookies[TokenCookieName] != null;

    public string? GetUsername() => _httpContext.Request.Cookies[UserCookieName];
}

public class LoginResult
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public List<string> Roles { get; set; } = new();
}

public class LoginResponse
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime ExpiryDate { get; set; }
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public List<string> Roles { get; set; } = new();
}