namespace Baselib.Business.DTOs;

using Baselib.Core.Enums;
using System.ComponentModel.DataAnnotations;



public class CreatePermissionDto
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(150)]
    public string Code { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required, StringLength(100)]
    public string? ControllerName { get; set; }

    [Required, StringLength(100)]
    public string? ActionName { get; set; }
    public CRUDActionType CRUDActionType { get; set; }
    public bool IsActive { get; set; } = true;
}
