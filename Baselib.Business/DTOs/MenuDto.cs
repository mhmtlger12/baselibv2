namespace Baselib.Business.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public List<MenuDto> SubMenus { get; set; } = new();
    public int Order { get; set; }
    public int? PermissionId { get; set; }
    public string? PermissionCode { get; set; }
}

public class CreateMenuDto
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public int Order { get; set; }
    public int? PermissionId { get; set; }
}

public class UpdateMenuDto
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public int Order { get; set; }
    public int? PermissionId { get; set; }
    public bool IsActive { get; set; }
}