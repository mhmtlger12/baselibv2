using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class StudyLevel : SoftDeleteEntity
{
    [Required, StringLength(80), RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$")] public string Key { get; set; } = "";
    [Required, StringLength(100)] public string Label { get; set; } = "";
    [Range(0, 100000)] public int SortOrder { get; set; }
}
