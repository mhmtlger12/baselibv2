namespace Baselib.Entities;

public class Menu : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public Menu? Parent { get; set; }
    public ICollection<Menu> SubMenus { get; set; } = new List<Menu>();
    public int Order { get; set; }
    public int? PermissionId { get; set; }
    public Permission? Permission { get; set; }
}