using System.ComponentModel.DataAnnotations;

namespace Baselib.Business.DTOs;

public class CreateMenuDto
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2048)]
    public string? Url { get; set; }

    [StringLength(100)]
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    [Range(0, 10000)] public int Order { get; set; }
    public int? PermissionId { get; set; }
}
