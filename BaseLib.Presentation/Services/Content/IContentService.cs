namespace BaseLib.Presentation.Services.Content;
public interface IContentService
{
    ContentPage? List(string slug);
    ContentEditModel? Edit(string slug, int id);
    IReadOnlyDictionary<string, string> Save(ContentEditModel model);
    bool Delete(string slug, int id);
}
