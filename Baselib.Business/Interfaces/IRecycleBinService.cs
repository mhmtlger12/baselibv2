using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface IRecycleBinService
{
    Task<IEnumerable<RecycleBinItemDto>> GetAllDeletedItemsAsync();
    Task RestoreAsync(string type, int id);
}
