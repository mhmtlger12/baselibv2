using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface ISettingService
{
    Task<IEnumerable<SettingDto>> GetAllAsync();
    Task<SettingDto?> GetByKeyAsync(string key);
    Task UpdateAsync(int id, UpdateSettingDto dto);
}
