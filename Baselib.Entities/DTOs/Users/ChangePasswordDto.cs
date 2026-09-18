using System.ComponentModel.DataAnnotations;
using Baselib.Core.Constants;

namespace Baselib.Business.DTOs;

public class ChangePasswordDto
{
    [Required, StringLength(PasswordPolicyConstants.MaximumLength)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, StringLength(PasswordPolicyConstants.MaximumLength, MinimumLength = PasswordPolicyConstants.MinimumLength)]
    public string NewPassword { get; set; } = string.Empty;
}
