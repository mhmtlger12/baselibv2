namespace Baselib.Business.Helpers;

/// <summary>
/// Şifre doğrulama ve hash işlemlerini merkezileştiren yardımcı sınıf.
/// </summary>
public static class PasswordHelper
{
    public static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool Verify(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }
}
