namespace Baselib.Business.Helpers;

public static class UserIdentityHelper
{
    public static string Normalize(string? value)
    {
        return value?.Trim().ToUpperInvariant() ?? string.Empty;
    }
}
