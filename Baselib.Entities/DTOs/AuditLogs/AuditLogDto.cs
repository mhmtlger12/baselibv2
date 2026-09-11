namespace Baselib.Business.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime CreatedDate { get; set; }
}
