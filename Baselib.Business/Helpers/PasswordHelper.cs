using Baselib.Core.Constants;

namespace Baselib.Business.Helpers;

/// <summary>
/// Şifre doğrulama ve hash işlemlerini merkezileştiren yardımcı sınıf.
/// </summary>
public static class PasswordHelper
{
    public const int MinimumPasswordLength = PasswordPolicyConstants.MinimumLength;
    public const int MaximumPasswordLength = PasswordPolicyConstants.MaximumLength;

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

    public static bool MeetsPolicy(string? password)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            password.Length < MinimumPasswordLength ||
            password.Length > MaximumPasswordLength)
        {
            return false;
        }

        return password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(character => !char.IsLetterOrDigit(character));
    }
}
