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
