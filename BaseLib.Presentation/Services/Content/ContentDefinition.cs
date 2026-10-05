using Baselib.Business.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace BaseLib.Presentation.Services.Content;

public sealed class ContentDefinition
{
    public string Title { get; init; } = "";
    public string Description { get; init; } = "İçerik kayıtlarını yönetin. Aktif kayıtlar sitede gösterilir.";
    public string AddLabel { get; init; } = "Yeni Kayıt";
    public IReadOnlyList<ContentColumn> Columns { get; init; } = [];
    public IReadOnlyList<ContentFieldDto> Fields { get; init; } = [];
}
public sealed record ContentColumn(string Key, string Label);
public sealed record ContentPage(string Slug, ContentDefinition Definition, IReadOnlyList<Dictionary<string, string?>> Rows);
public sealed class ContentEditModel
{
    public string Slug { get; set; } = "";
    public int Id { get; set; }
    public Dictionary<string, string?> Values { get; set; } = [];
    [ValidateNever] public ContentDefinition Definition { get; set; } = new();
}
