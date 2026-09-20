using System.ComponentModel.DataAnnotations;

namespace Baselib.Entities.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class WebAddressAttribute : ValidationAttribute
{
    public WebAddressAttribute() : base("Yerel bir yol veya http/https adresi girin.") { }

    public override bool IsValid(object? value)
    {
        if (value is null || value is string { Length: 0 }) return true;
        if (value is not string address || address.Any(char.IsControl) || address.Contains('\\')) return false;
        if (address.StartsWith('/') && !address.StartsWith("//")) return true;
        return Uri.TryCreate(address, UriKind.Absolute, out var uri)
            && uri.Scheme is "http" or "https" && !string.IsNullOrEmpty(uri.Host);
    }
}
