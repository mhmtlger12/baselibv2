using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Entities;

public sealed class SiteAdvertisement : SoftDeleteEntity
{
    [Required, StringLength(200)] public string Name { get; set; } = "";
    [Required, StringLength(40)] public string Position { get; set; } = "";
    [StringLength(2048), WebAddress] public string ImageUrl { get; set; } = "";
    [StringLength(2048), WebAddress] public string LinkUrl { get; set; } = "";
}
