using System.ComponentModel.DataAnnotations;
using Baselib.Entities.Validation;

namespace Baselib.Business.DTOs;

public class SaveSliderDto
{
    [Required(ErrorMessage = "Başlık zorunludur."), StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Görsel adresi zorunludur."), StringLength(2048), WebAddress]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(2048), WebAddress]
    public string? LinkUrl { get; set; }

    [Range(0, 10000, ErrorMessage = "Sıra 0 ile 10000 arasında olmalıdır.")]
    public int Order { get; set; }

    public bool IsActive { get; set; } = true;
}
