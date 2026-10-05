using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class ScorePeriod : SoftDeleteEntity
{
    [Required, StringLength(40)] public string Name { get; set; } = "";
    [Range(0, 100000)] public int SortOrder { get; set; }
}
