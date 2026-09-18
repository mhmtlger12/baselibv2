namespace Baselib.Api.Attributes;

/// <summary>
/// Bir yönetim endpointinin erişim için zorunlu tuttuğu sabit permission kodunu tanımlar.
/// Metot adları veya route değerleri yetki kararında kullanılmaz.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequirePermissionAttribute : Attribute
{
    public RequirePermissionAttribute(string code)
    {
        Code = code;
    }

    public string Code { get; }
}
