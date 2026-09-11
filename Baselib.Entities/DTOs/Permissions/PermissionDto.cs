namespace Baselib.Business.DTOs;

using Baselib.Core.Enums;



public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public CRUDActionType CRUDActionType { get; set; }
    public bool IsActive { get; set; }
}
