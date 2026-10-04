using Baselib.Business.DTOs;

namespace BaseLib.Presentation.Services.Api;

public interface IPublicSiteSettingsApiService
{
    Task<PublicSiteSettingsDto> GetAsync(CancellationToken ct);
}

// Scoped lifetime: head, header and footer share one request; the next page reads fresh settings.
public sealed class PublicSiteSettingsApiService(ApiTransport transport) : IPublicSiteSettingsApiService
{
    private Task<PublicSiteSettingsDto>? settings;

    public Task<PublicSiteSettingsDto> GetAsync(CancellationToken ct) => settings ??= LoadAsync(ct);

    private async Task<PublicSiteSettingsDto> LoadAsync(CancellationToken ct)
    {
        try
        {
            return await transport.SendAsync<PublicSiteSettingsDto>(HttpMethod.Get, ApiRoutes.PublicSiteSettings, cancellationToken: ct);
        }
        catch (ApiException)
        {
            // Keep login and error pages renderable when the API is unavailable.
            return new PublicSiteSettingsDto { Name = "Site" };
        }
    }
}
