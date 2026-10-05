namespace BaseLib.Presentation.Models.Site;
public sealed record JobCategory(string Key, string Label, List<JobCategory>? Children);
public sealed record JobListing(int Id, string Institution, string Summary, string CategoryKey, string CategoryLabel, DateTime PublishedAt, string StartDate, string EndDate, string? SourceUrl = null, string? PdfUrl = null, string? InstitutionLogoUrl = null)
{
    public static JobListing FromDto(Baselib.Business.DTOs.JobListingDto dto) =>
        new(dto.Id, dto.Institution, dto.Summary, dto.CategoryKey, dto.CategoryLabel, dto.PublishedAt,
            DisplayDate(dto.StartDate), DisplayDate(dto.EndDate), dto.SourceUrl, dto.PdfUrl, dto.InstitutionLogoUrl);

    private static string DisplayDate(string value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var date)
            ? date.ToString("d MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("tr-TR")) : value;
}
