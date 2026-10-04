using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Baselib.Business.DTOs;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class UpdateUserDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(32)]
    public string? Phone { get; set; }
    public int? DepartmentId { get; set; }

    public bool IsActive { get; set; }
}
