namespace Baselib.Business.DTOs;

// Only branding values are exposed publicly; internal settings stay protected.
public sealed class PublicSiteSettingsDto
{
    public string Name { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
}
