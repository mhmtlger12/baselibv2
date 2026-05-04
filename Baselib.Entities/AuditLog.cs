namespace Baselib.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
