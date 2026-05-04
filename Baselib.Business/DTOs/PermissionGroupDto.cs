namespace Baselib.Business.DTOs;

public class PermissionGroupDto
{
    public string ControllerName { get; set; } = string.Empty;
    public bool Checked { get; set; }
    public bool Indeterminate { get; set; }
    public List<ControllerCrudDto> ControllerCrudList { get; set; } = new();
}

public class ControllerCrudDto
{
    public int PermissionId { get; set; }
    public int CRUDActionType { get; set; }
    public bool Checked { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public static class CRUDActionTypes
{
    public const int View = 1;
    public const int Add = 2;
    public const int Update = 3;
    public const int Preview = 4;
    public const int Option = 5;
    public const int Delete = 6;

    public static string GetName(int value) => value switch
    {
        View => "View",
        Add => "Add",
        Update => "Update",
        Preview => "Preview",
        Option => "Option",
        Delete => "Delete",
        _ => value.ToString()
    };
}
