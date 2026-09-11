using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IDashboardService
{
    Task<IDataResult<DashboardStatsDto>> GetStatsAsync(CancellationToken cancellationToken = default);
}
