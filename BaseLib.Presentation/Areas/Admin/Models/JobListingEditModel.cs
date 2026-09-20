using Baselib.Business.DTOs;

namespace BaseLib.Presentation.Areas.Admin.Models;

public sealed class JobListingEditModel : SaveJobListingDto
{
    public int Id { get; set; }
    public IReadOnlyList<Baselib.Business.DTOs.InstitutionDto> Institutions { get; set; } = [];
}
