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
    public int CRUDActionType { get; set; }
    public bool Checked { get; set; }
    public string Name { get; set; } = string.Empty;
}

public static class CRUDActionTypes
{
    public const int Create = 1;
    public const int Read = 2;
    public const int Update = 4;
    public const int Delete = 8;

    public static string GetName(int value) => value switch
    {
        Create => "Ekleme",
        Read => "Okuma",
        Update => "Güncelleme",
        Delete => "Silme",
        _ => value.ToString()
    };
}