namespace Baselib.Entities;

// Inactive records remain manageable; only explicitly deleted records enter the recycle bin.
public abstract class SoftDeleteEntity : BaseEntity
{
    public bool IsDeleted { get; set; }
}
