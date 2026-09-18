namespace Baselib.Core.Constants;

/// <summary>
/// DTO doğrulaması ve iş kuralının birlikte kullandığı parola sınırları.
/// Core katmanında tutulur; Entities -> Business bağımlılığı oluşmaz.
/// </summary>
public static class PasswordPolicyConstants
{
    public const int MinimumLength = 12;
    public const int MaximumLength = 128;
}
