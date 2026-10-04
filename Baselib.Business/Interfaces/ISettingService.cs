using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface ISettingService
{
    Task<IDataResult<IEnumerable<SettingDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<PublicSiteSettingsDto>> GetPublicAsync(CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(int id, UpdateSettingDto dto, CancellationToken cancellationToken = default);
}
