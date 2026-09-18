using System.ComponentModel.DataAnnotations;

namespace Baselib.Business.DTOs;

public class UpdateSettingDto
{
    [Required, StringLength(4000)]
    public string Value { get; set; } = string.Empty;
}
