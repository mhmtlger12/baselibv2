namespace Baselib.Business.DTOs;

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public int? DepartmentId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}
