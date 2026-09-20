using System.Text.Json.Nodes;
namespace BaseLib.Presentation.Services.Content;
public sealed class ContentDefinition
{
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public string AddLabel { get; init; } = "";
    public List<ContentColumn> Columns { get; init; } = [];
    public List<ContentField> Fields { get; init; } = [];
    public List<JsonObject> Rows { get; set; } = [];
}
public sealed record ContentColumn(string Key, string Label);
public sealed record ContentField(string Key, string Label, string Type, string? Hint, List<string>? Options, bool Full);
public sealed record ContentPage(string Slug, ContentDefinition Definition, IReadOnlyList<JsonObject> Rows);
public sealed class ContentEditModel
{
    public string Slug { get; set; } = "";
    public int Id { get; set; }
    public Dictionary<string, string?> Values { get; set; } = [];
    [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public ContentDefinition Definition { get; set; } = new();
}
