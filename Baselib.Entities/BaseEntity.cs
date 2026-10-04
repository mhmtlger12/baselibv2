using Baselib.Core.Interfaces;

namespace Baselib.Entities;

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;
}