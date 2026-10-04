using Baselib.Business.DTOs;
using Baselib.Core.Enums;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IRecycleBinService
{
    Task<IDataResult<IEnumerable<RecycleBinItemDto>>> GetAllDeletedItemsAsync(CancellationToken cancellationToken = default);
    Task<IResult> RestoreAsync(RecycleBinType type, int id, CancellationToken cancellationToken = default);
    Task<IResult> RestoreAsync(string type, int id, CancellationToken cancellationToken = default);
}
