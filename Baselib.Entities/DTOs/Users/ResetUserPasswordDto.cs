using System.ComponentModel.DataAnnotations;
using Baselib.Core.Constants;

namespace Baselib.Business.DTOs;

public class ResetUserPasswordDto
{
    [Required, StringLength(PasswordPolicyConstants.MaximumLength, MinimumLength = PasswordPolicyConstants.MinimumLength)]
    public string NewPassword { get; set; } = string.Empty;
}
