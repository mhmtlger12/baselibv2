using System.Globalization;
using Baselib.Business.DTOs;
using BaseLib.Presentation.Services.Api;
namespace BaseLib.Presentation.Services.Content;

public sealed class ContentApiService(IApiClient api) : IContentService
{
    private static string Route(string slug) => "api/content/" + Uri.EscapeDataString(slug);
    public async Task<ContentPage?> ListAsync(string slug, CancellationToken ct)
    {
        ContentModuleDto module;
        try { module = await api.GetAsync<ContentModuleDto>(Route(slug), ct); }
        catch (ApiException error) when (error.StatusCode == 404) { return null; }
        return new(slug, new ContentDefinition { Title = module.Title, Fields = module.Fields,
            Columns = module.Fields.Where(x => x.Type != "textarea").Take(6).Select(x => new ContentColumn(x.Key, x.Label)).ToList() }, module.Rows);
    }
    public async Task<ContentEditModel?> EditAsync(string slug, int id, CancellationToken ct)
    {
        var page = await ListAsync(slug, ct);
        if (page is null) return null;
        var row = page.Rows.FirstOrDefault(x => x["id"] == id.ToString(CultureInfo.InvariantCulture));
        if (id != 0 && row is null) return null;
        return new() { Id = id, Slug = slug, Definition = page.Definition, Values = page.Definition.Fields.ToDictionary(x => x.Key,
            x => row?.GetValueOrDefault(x.Key) ?? (x.Type == "checkbox" ? x.Key is "active" or "selectable" ? "true" : "false" : x.Type == "number" ? "0" : x.Type == "date" ? DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : ""))! };
    }
    public Task SaveAsync(ContentEditModel model, CancellationToken ct) => api.SendAsync(model.Id == 0 ? HttpMethod.Post : HttpMethod.Put,
        model.Id == 0 ? Route(model.Slug) : Route(model.Slug) + "/" + model.Id.ToString(CultureInfo.InvariantCulture), new SaveContentDto { Values = model.Values }, ct);
    public Task DeleteAsync(string slug, int id, CancellationToken ct) => api.SendAsync(HttpMethod.Delete, Route(slug) + "/" + id.ToString(CultureInfo.InvariantCulture), ct: ct);
}
