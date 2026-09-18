using System.ComponentModel.DataAnnotations;
using Baselib.Core.Constants;

namespace Baselib.Business.DTOs;

public class LoginDto
{
    [Required, StringLength(254)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(PasswordPolicyConstants.MaximumLength)]
    public string Password { get; set; } = string.Empty;
}
