namespace Baselib.Business.DTOs;

public class PermissionGroupDto
{
    public string ControllerName { get; set; } = string.Empty;
    public bool Checked { get; set; }
    public bool Indeterminate { get; set; }
    public List<ControllerCrudDto> ControllerCrudList { get; set; } = new();
}
