using System.ComponentModel.DataAnnotations;
using Baselib.Business.Helpers;

namespace Baselib.Business.DTOs;

public class LoginDto
{
    [Required, StringLength(254)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(PasswordHelper.MaximumPasswordLength)]
    public string Password { get; set; } = string.Empty;
}
