using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface ISettingService
{
    Task<IDataResult<IEnumerable<SettingDto>>> GetAllAsync();
    Task<IDataResult<SettingDto>> GetByKeyAsync(string key);
    Task<IResult> UpdateAsync(int id, UpdateSettingDto dto);
}
