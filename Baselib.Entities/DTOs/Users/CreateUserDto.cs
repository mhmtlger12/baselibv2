using System.ComponentModel.DataAnnotations;
using Baselib.Business.Helpers;

namespace Baselib.Business.DTOs;

public class CreateUserDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(PasswordHelper.MaximumPasswordLength, MinimumLength = PasswordHelper.MinimumPasswordLength)]
    public string Password { get; set; } = string.Empty;

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(32)]
    public string? Phone { get; set; }
    public int? DepartmentId { get; set; }

    [MaxLength(100)]
    public List<int> RoleIds { get; set; } = new();
}
