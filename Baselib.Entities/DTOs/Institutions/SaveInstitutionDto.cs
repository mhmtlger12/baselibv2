using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Business.DTOs;

public class SaveInstitutionDto
{
    [Required, StringLength(200)] public string Name { get; set; } = string.Empty;
    [StringLength(2048), WebAddress] public string? LogoUrl { get; set; }
    [StringLength(2048), WebAddress] public string? WebsiteUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
