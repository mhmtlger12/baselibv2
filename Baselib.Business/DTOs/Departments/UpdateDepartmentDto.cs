namespace Baselib.Business.DTOs;

public class UpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? ParentDepartmentId { get; set; }
    public bool IsActive { get; set; }
}
