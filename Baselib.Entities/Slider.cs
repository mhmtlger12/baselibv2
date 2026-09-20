namespace Baselib.Entities;

public class Slider : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public int Order { get; set; }
    public bool IsDeleted { get; set; }
}
