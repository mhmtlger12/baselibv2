using Baselib.Business.DTOs;

namespace BaseLib.Presentation.Services.Api;

public interface IPublicSliderApiService
{
    Task<List<SliderDto>> GetPublishedAsync(CancellationToken ct);
}

public sealed class PublicSliderApiService(ApiTransport transport) : IPublicSliderApiService
{
    public Task<List<SliderDto>> GetPublishedAsync(CancellationToken ct) =>
        transport.SendAsync<List<SliderDto>>(HttpMethod.Get, ApiRoutes.PublishedSliders, cancellationToken: ct);
}
