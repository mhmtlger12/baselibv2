using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class PublicComment : SoftDeleteEntity
{
    [Required, StringLength(100)] public string Author { get; set; } = "";
    [StringLength(254), EmailAddress] public string? Email { get; set; }
    [Required, StringLength(4000)] public string Body { get; set; } = "";
    [Required, StringLength(200)] public string Page { get; set; } = "";
    [Required, StringLength(20)] public string Status { get; set; } = "Beklemede";
    public int? ParentId { get; set; }
    public PublicComment? Parent { get; set; }
    public bool HideName { get; set; }
    [Range(0,int.MaxValue)] public int Likes { get; set; }
}
