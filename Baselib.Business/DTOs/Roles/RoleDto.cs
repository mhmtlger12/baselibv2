namespace Baselib.Business.DTOs;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
    public int PermissionCount => Permissions.Count;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}
