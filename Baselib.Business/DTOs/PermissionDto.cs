namespace Baselib.Business.DTOs;

public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ControllerName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public int CRUDActionType { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePermissionDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public int CRUDActionType { get; set; }
    public bool IsActive { get; set; } = true;
}
