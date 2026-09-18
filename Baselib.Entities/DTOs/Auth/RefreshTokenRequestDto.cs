using System.ComponentModel.DataAnnotations;

namespace Baselib.Business.DTOs;

public class RefreshTokenRequestDto
{
    [Required, StringLength(512)]
    public string RefreshToken { get; set; } = string.Empty;
}
