namespace Baselib.Business.DTOs;

using Baselib.Core.Enums;



public class ControllerCrudDto
{
    public int PermissionId { get; set; }
    public CRUDActionType CRUDActionType { get; set; }
    public bool Checked { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
