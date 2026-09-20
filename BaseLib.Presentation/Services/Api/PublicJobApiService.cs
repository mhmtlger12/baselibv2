using Baselib.Business.DTOs;

namespace BaseLib.Presentation.Services.Api;

public interface IPublicJobApiService
{
    Task<List<JobListingDto>> GetPublishedAsync(string? categoryKey, string? query, CancellationToken ct);
    Task<JobListingDto?> GetAsync(int id, CancellationToken ct);
}

public sealed class PublicJobApiService(ApiTransport transport) : IPublicJobApiService
{
    public Task<List<JobListingDto>> GetPublishedAsync(string? categoryKey, string? query, CancellationToken ct) =>
        transport.SendAsync<List<JobListingDto>>(HttpMethod.Get,
            $"{ApiRoutes.PublishedJobs}?categoryKey={Uri.EscapeDataString(categoryKey ?? "")}&q={Uri.EscapeDataString(query ?? "")}", cancellationToken: ct);

    public async Task<JobListingDto?> GetAsync(int id, CancellationToken ct)
    {
        try { return await transport.SendAsync<JobListingDto>(HttpMethod.Get, ApiRoutes.Item(ApiRoutes.Jobs, id), cancellationToken: ct); }
        catch (ApiException error) when (error.StatusCode == 404) { return null; }
    }
}
