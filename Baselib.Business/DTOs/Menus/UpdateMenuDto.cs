namespace Baselib.Business.DTOs;

public class UpdateMenuDto
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public int Order { get; set; }
    public int? PermissionId { get; set; }
    public bool IsActive { get; set; }
}
