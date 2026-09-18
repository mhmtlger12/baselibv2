namespace Baselib.Business.DTOs;

/// <summary>
/// Refresh token oturumunun geldiği istemciye ait, güvenlik/audit amaçlı bilgiler.
/// </summary>
public sealed class ClientSessionInfoDto
{
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}
