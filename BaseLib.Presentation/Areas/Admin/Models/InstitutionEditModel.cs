using Baselib.Business.DTOs;

namespace BaseLib.Presentation.Areas.Admin.Models;

public sealed class InstitutionEditModel : SaveInstitutionDto
{
    public int Id { get; set; }
}
