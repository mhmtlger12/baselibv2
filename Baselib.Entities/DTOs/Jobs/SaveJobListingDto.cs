using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Baselib.Entities.Validation;

namespace Baselib.Business.DTOs;

public class SaveJobListingDto : IValidatableObject
{
    public int? InstitutionId { get; set; }
    [StringLength(200)] public string? Institution { get; set; } = string.Empty;
    [Required, StringLength(500)] public string Summary { get; set; } = string.Empty;
    [Required, StringLength(80)] public string CategoryKey { get; set; } = string.Empty;
    [StringLength(150)] public string? CategoryLabel { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow.Date;
    [Required, StringLength(80)] public string StartDate { get; set; } = string.Empty;
    [Required, StringLength(80)] public string EndDate { get; set; } = string.Empty;
    [StringLength(2048), WebAddress] public string? SourceUrl { get; set; }
    [StringLength(2048), WebAddress] public string? PdfUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var startValid = DateOnly.TryParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start);
        var endValid = DateOnly.TryParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var end);
        if (!startValid) yield return new("Başvuru başlangıcı yıl içeren geçerli bir tarih olmalıdır (yyyy-MM-dd).", [nameof(StartDate)]);
        if (!endValid) yield return new("Başvuru bitişi yıl içeren geçerli bir tarih olmalıdır (yyyy-MM-dd).", [nameof(EndDate)]);
        if (startValid && endValid && end < start)
            yield return new("Başvuru bitişi başlangıç tarihinden önce olamaz.", [nameof(EndDate)]);
        if (PublishedAt == default)
            yield return new("Yayın tarihi zorunludur.", [nameof(PublishedAt)]);
    }
}
