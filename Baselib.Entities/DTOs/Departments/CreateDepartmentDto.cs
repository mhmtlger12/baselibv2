using System.ComponentModel.DataAnnotations;

namespace Baselib.Business.DTOs;

public class CreateDepartmentDto
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Code { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
}
