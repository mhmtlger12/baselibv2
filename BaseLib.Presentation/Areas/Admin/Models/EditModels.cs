using System.ComponentModel.DataAnnotations;
using Baselib.Business.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace BaseLib.Presentation.Areas.Admin.Models;
public sealed class UserEditModel : UpdateUserDto, IValidatableObject
{
    public int Id { get; set; }
    [MaxLength(100)] public List<int> RoleIds { get; set; } = [];
    [ValidateNever] public List<RoleDto> AvailableRoles { get; set; } = [];
    [ValidateNever] public List<DepartmentDto> Departments { get; set; } = [];
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Id == 0 && string.IsNullOrEmpty(Password)) yield return new("Yeni kullanıcı için şifre zorunludur.", [nameof(Password)]);
        if (!string.IsNullOrEmpty(Password) && (Password.Length < 12 || !Password.Any(char.IsUpper) || !Password.Any(char.IsLower) || !Password.Any(char.IsDigit) || !Password.Any(x => !char.IsLetterOrDigit(x))))
            yield return new("Şifre en az 12 karakter; büyük harf, küçük harf, rakam ve özel karakter içermelidir.", [nameof(Password)]);
    }
}
public sealed class RoleEditModel : UpdateRoleDto
{
    public int Id { get; set; }
    [ValidateNever] public List<PermissionDto> AvailablePermissions { get; set; } = [];
}
public sealed class PermissionEditModel : CreatePermissionDto, IValidatableObject
{
    public int Id { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Enum.IsDefined(CRUDActionType)) yield return new("Geçerli bir işlem türü seçin.", [nameof(CRUDActionType)]);
    }
}
public sealed class DepartmentEditModel : UpdateDepartmentDto
{
    public int Id { get; set; }
    [ValidateNever] public List<DepartmentDto> Departments { get; set; } = [];
}
public sealed class MenuEditModel : UpdateMenuDto
{
    public int Id { get; set; }
    [ValidateNever] public List<MenuDto> Menus { get; set; } = [];
    [ValidateNever] public List<PermissionDto> Permissions { get; set; } = [];
}
public sealed class PasswordModel : ChangePasswordDto
{
    [Required, Compare(nameof(NewPassword), ErrorMessage = "Yeni şifreler birbiriyle eşleşmiyor.")]
    public string Confirmation { get; set; } = "";
}
public sealed class ProfileModel
{
    public UserDto User { get; set; } = new();
    public PasswordModel Password { get; set; } = new();
}
public sealed record DashboardModel(DashboardStatsDto Stats, IReadOnlyList<AuditLogDto> Logs, string? LogsError);
public sealed record DeleteModel(int Id, string Name);
public sealed class SettingEditModel : UpdateSettingDto
{
    public int Id { get; set; }
    [ValidateNever] public string Key { get; set; } = "";
    [ValidateNever] public string Description { get; set; } = "";
}
public sealed record NavigationModel(IReadOnlySet<string> AllowedUrls, string? Error);
