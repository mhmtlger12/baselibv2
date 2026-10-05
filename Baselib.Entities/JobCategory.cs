using System.ComponentModel.DataAnnotations;

namespace Baselib.Entities;

public sealed class JobCategory : SoftDeleteEntity
{
    [Required, StringLength(80)] public string Key { get; set; } = string.Empty;
    [Required, StringLength(150)] public string Label { get; set; } = string.Empty;
    [StringLength(80)] public string? ParentKey { get; set; }
    [Range(0, 100000)] public int SortOrder { get; set; }
    public bool IsSelectable { get; set; } = true;
}
