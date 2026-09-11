namespace Baselib.Core.Enums;

public enum RecycleBinType
{
    User = 1,
    Role = 2,
    Department = 3
}

public static class RecycleBinTypeExtensions
{
    public static string GetDisplayName(this RecycleBinType type) => type switch
    {
        RecycleBinType.User => "Kullanıcı",
        RecycleBinType.Role => "Rol",
        RecycleBinType.Department => "Departman",
        _ => type.ToString()
    };

    public static bool TryParse(string? value, out RecycleBinType result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return false;
        }

        if (Enum.TryParse<RecycleBinType>(value, ignoreCase: true, out result) && Enum.IsDefined(result))
            return true;

        switch (value.Trim().ToLowerInvariant())
        {
            case "kullanıcı":
            case "kullanici":
            case "user":
                result = RecycleBinType.User;
                return true;
            case "rol":
            case "role":
                result = RecycleBinType.Role;
                return true;
            case "departman":
            case "department":
                result = RecycleBinType.Department;
                return true;
            default:
                result = default;
                return false;
        }
    }
}
