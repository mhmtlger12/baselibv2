namespace Baselib.Entities;

public sealed class JobListing : BaseEntity
{
    public int? InstitutionId { get; set; }
    public Institution? InstitutionEntity { get; set; }
    public string Institution { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string CategoryKey { get; set; } = string.Empty;
    public string CategoryLabel { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public string? PdfUrl { get; set; }
    public bool IsDeleted { get; set; }
}
