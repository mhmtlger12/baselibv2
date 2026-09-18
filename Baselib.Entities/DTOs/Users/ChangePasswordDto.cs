using System.ComponentModel.DataAnnotations;
using Baselib.Business.Helpers;

namespace Baselib.Business.DTOs;

public class ChangePasswordDto
{
    [Required, StringLength(PasswordHelper.MaximumPasswordLength)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, StringLength(PasswordHelper.MaximumPasswordLength, MinimumLength = PasswordHelper.MinimumPasswordLength)]
    public string NewPassword { get; set; } = string.Empty;
}
