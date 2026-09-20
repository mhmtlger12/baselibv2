using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Business.DTOs;

public class SaveJobListingDto
{
    public int? InstitutionId { get; set; }
    [Required, StringLength(200)] public string Institution { get; set; } = string.Empty;
    [Required, StringLength(500)] public string Summary { get; set; } = string.Empty;
    [Required, StringLength(80)] public string CategoryKey { get; set; } = string.Empty;
    [Required, StringLength(150)] public string CategoryLabel { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow.Date;
    [Required, StringLength(80)] public string StartDate { get; set; } = string.Empty;
    [Required, StringLength(80)] public string EndDate { get; set; } = string.Empty;
    [StringLength(2048), WebAddress] public string? SourceUrl { get; set; }
    [StringLength(2048), WebAddress] public string? PdfUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
