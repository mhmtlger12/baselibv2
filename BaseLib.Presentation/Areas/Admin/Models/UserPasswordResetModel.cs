using System.ComponentModel.DataAnnotations;
using Baselib.Business.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BaseLib.Presentation.Areas.Admin.Models;

public sealed class UserPasswordResetModel : ResetUserPasswordDto
{
    public int Id { get; set; }
    [ValidateNever] public string Username { get; set; } = string.Empty;
    [Required, Compare(nameof(NewPassword), ErrorMessage = "Yeni şifreler birbiriyle eşleşmiyor.")]
    public string Confirmation { get; set; } = string.Empty;
}
