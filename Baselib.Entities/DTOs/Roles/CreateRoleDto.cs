using System.ComponentModel.DataAnnotations;

namespace Baselib.Business.DTOs;

public class CreateRoleDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public List<int> PermissionIds { get; set; } = new();
}
