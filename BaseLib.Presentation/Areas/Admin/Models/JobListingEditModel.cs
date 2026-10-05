using Baselib.Business.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BaseLib.Presentation.Areas.Admin.Models;

public sealed class JobListingEditModel : SaveJobListingDto
{
    public int Id { get; set; }
    [ValidateNever] public IReadOnlyList<JobCategoryDto> Categories { get; set; } = [];
    [ValidateNever] public string? LegacyDates { get; set; }
    [ValidateNever] public IReadOnlyList<Baselib.Business.DTOs.InstitutionDto> Institutions { get; set; } = [];
}
