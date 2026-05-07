namespace Baselib.Core.Enums;

/// <summary>
/// CRUD işlem tiplerini tanımlayan sabitler.
/// Permission sistemi tarafından kullanılır.
/// </summary>
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
