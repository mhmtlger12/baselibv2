using System.ComponentModel.DataAnnotations;
using Baselib.Core.Constants;

namespace Baselib.Business.DTOs;

/// <summary>
/// Anonim kullanıcı kaydı için istek modeli.
/// Rol ve departman ataması yalnızca yönetim akışında yapılır.
/// </summary>
public class RegisterUserDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(PasswordPolicyConstants.MaximumLength, MinimumLength = PasswordPolicyConstants.MinimumLength)]
    public string Password { get; set; } = string.Empty;

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(32)]
    public string? Phone { get; set; }
}
