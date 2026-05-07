namespace Baselib.Business.DTOs;

public class AuthResultDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public UserDto User { get; set; } = null!;
}
