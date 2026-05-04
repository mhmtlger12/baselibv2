namespace Baselib.Business.DTOs;

public class RecycleBinItemDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // "User", "Role", "Department" vs.
    public string Name { get; set; } = string.Empty; // Silinen nesnenin adı veya açıklaması
    public DateTime? DeletedDate { get; set; }
}
