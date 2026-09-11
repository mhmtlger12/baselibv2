namespace Baselib.Business.DTOs;

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
}
