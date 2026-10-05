using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class ExamCountdown : SoftDeleteEntity
{
    [Required, StringLength(100)] public string Label { get; set; } = "";
    public DateTime Target { get; set; }
    [Range(0, 100000)] public int SortOrder { get; set; }
}
