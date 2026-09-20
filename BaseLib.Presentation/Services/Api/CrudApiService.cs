namespace BaseLib.Presentation.Services.Api;
public interface ICrudApiService<TRead, in TCreate, in TUpdate>
{
    Task<List<TRead>> ListAsync(CancellationToken ct);
    Task<TRead> GetAsync(int id, CancellationToken ct);
    Task CreateAsync(TCreate input, CancellationToken ct);
    Task UpdateAsync(int id, TUpdate input, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}
public sealed class CrudApiService<TRead, TCreate, TUpdate>(IApiClient api, string resource) : ICrudApiService<TRead, TCreate, TUpdate>
{
    public Task<List<TRead>> ListAsync(CancellationToken ct) => api.GetAsync<List<TRead>>(resource, ct);
    public Task<TRead> GetAsync(int id, CancellationToken ct) => api.GetAsync<TRead>(ApiRoutes.Item(resource, id), ct);
    public Task CreateAsync(TCreate input, CancellationToken ct) => api.SendAsync(HttpMethod.Post, resource, input, ct);
    public Task UpdateAsync(int id, TUpdate input, CancellationToken ct) => api.SendAsync(HttpMethod.Put, ApiRoutes.Item(resource, id), input, ct);
    public Task DeleteAsync(int id, CancellationToken ct) => api.SendAsync(HttpMethod.Delete, ApiRoutes.Item(resource, id), ct: ct);
}
