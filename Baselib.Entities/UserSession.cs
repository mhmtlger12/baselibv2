using Baselib.Core.Interfaces;

namespace Baselib.Entities;

public sealed class UserSession : IEntity
{
    public int Id { get; set; }
    public string FamilyId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime? RevokedDate { get; set; }
    public string? RevokedReason { get; set; }
}
