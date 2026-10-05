using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class SiteNavigation : SoftDeleteEntity
{
    [Required, StringLength(100)] public string Label { get; set; } = "";
    [Required, StringLength(2048), WebAddress] public string Url { get; set; } = "";
    [Range(0, 100000)] public int SortOrder { get; set; }
}
