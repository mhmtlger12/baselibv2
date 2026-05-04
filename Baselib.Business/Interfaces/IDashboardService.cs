using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}
