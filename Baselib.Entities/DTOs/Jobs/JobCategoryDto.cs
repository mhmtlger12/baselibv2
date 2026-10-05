namespace Baselib.Business.DTOs;

public sealed record JobCategoryDto(string Key, string Label, string? ParentKey, int SortOrder, bool IsSelectable);
