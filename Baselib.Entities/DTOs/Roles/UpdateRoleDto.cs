namespace Baselib.Business.DTOs;

public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<int> PermissionIds { get; set; } = new();
    public bool IsActive { get; set; }
}
