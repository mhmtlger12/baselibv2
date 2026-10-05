using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class SiteNews : SoftDeleteEntity
{
    [Required, StringLength(30)] public string Section { get; set; } = "";
    [Required, StringLength(300)] public string Title { get; set; } = "";
    [StringLength(100)] public string Category { get; set; } = "";
    [Required, StringLength(2048), WebAddress] public string Link { get; set; } = "";
    public DateTime PublishedAt { get; set; }
    [Range(0, 100000)] public int SortOrder { get; set; }
}
