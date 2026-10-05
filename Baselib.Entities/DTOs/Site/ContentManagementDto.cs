namespace Baselib.Business.DTOs;

public sealed record ContentOptionDto(string Value, string Label);
public sealed record ContentFieldDto(string Key, string Label, string Type, bool Required, int MaxLength, IReadOnlyList<ContentOptionDto>? Options = null, string? Hint = null, bool Immutable = false);
public sealed record ContentModuleDto(string Slug, string Title, IReadOnlyList<ContentFieldDto> Fields, IReadOnlyList<Dictionary<string, string?>> Rows);
public sealed class SaveContentDto
{
    public Dictionary<string, string?> Values { get; set; } = [];
}
