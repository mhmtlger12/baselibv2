namespace Baselib.Core.Enums;

/// <summary>
/// CRUD işlem tiplerini tanımlayan enum.
/// Permission sistemi tarafından kullanılır.
/// </summary>
public enum CRUDActionType
{
    View = 1,
    Add = 2,
    Update = 3,
    Preview = 4,
    Option = 5,
    Delete = 6
}

public static class CRUDActionTypeExtensions
{
    public static string GetName(this CRUDActionType actionType) => actionType switch
    {
        CRUDActionType.View => "View",
        CRUDActionType.Add => "Add",
        CRUDActionType.Update => "Update",
        CRUDActionType.Preview => "Preview",
        CRUDActionType.Option => "Option",
        CRUDActionType.Delete => "Delete",
        _ => actionType.ToString()
    };
}

public static class CRUDActionTypes
{
    public const int View = 1;
    public const int Add = 2;
    public const int Update = 3;
    public const int Preview = 4;
    public const int Option = 5;
    public const int Delete = 6;

    public static string GetName(CRUDActionType value) => value.GetName();
    public static string GetName(int value) => ((CRUDActionType)value).GetName();
}
