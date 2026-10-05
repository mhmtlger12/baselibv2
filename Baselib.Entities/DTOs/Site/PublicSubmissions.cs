using System.ComponentModel.DataAnnotations;
namespace Baselib.Business.DTOs;

public class SendContactDto
{
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, StringLength(200)] public string Subject { get; set; } = "";
    [Required, StringLength(4000)] public string Message { get; set; } = "";
}
public sealed class SendCommentDto
{
    [Required, StringLength(100)] public string Author { get; set; } = "";
    [Required, EmailAddress, StringLength(254)] public string Email { get; set; } = "";
    [Required, StringLength(4000)] public string Body { get; set; } = "";
    [Required, StringLength(200)] public string Page { get; set; } = "";
    public int? ParentId { get; set; }
    public bool HideName { get; set; }
}
