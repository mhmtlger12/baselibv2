using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface ISettingService
{
    Task<IDataResult<IEnumerable<SettingDto>>> GetAllAsync();
    Task<IResult> UpdateAsync(int id, UpdateSettingDto dto);
}
