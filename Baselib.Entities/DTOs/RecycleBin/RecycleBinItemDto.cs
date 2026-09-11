namespace Baselib.Business.DTOs;

public class RecycleBinItemDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime? DeletedDate { get; set; }
}
