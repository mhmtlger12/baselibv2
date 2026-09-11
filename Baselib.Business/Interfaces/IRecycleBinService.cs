using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IRecycleBinService
{
    Task<IDataResult<IEnumerable<RecycleBinItemDto>>> GetAllDeletedItemsAsync();
    Task<IResult> RestoreAsync(string type, int id);
}
