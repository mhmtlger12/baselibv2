namespace BaseLib.Presentation.Services.Content;
public interface IContentService
{
    Task<ContentPage?> ListAsync(string slug, CancellationToken ct);
    Task<ContentEditModel?> EditAsync(string slug, int id, CancellationToken ct);
    Task SaveAsync(ContentEditModel model, CancellationToken ct);
    Task DeleteAsync(string slug, int id, CancellationToken ct);
}
