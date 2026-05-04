using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class SettingService : ISettingService
{
    private readonly IRepository<AppSetting> _settings;
    private readonly IUnitOfWork _unitOfWork;

    public SettingService(IRepository<AppSetting> settings, IUnitOfWork unitOfWork)
    {
        _settings = settings;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SettingDto>> GetAllAsync()
    {
        var settings = await _settings.Query()
            .OrderBy(s => s.Key)
            .ToListAsync();

        return settings.Select(s => new SettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Description = s.Description
        });
    }

    public async Task<SettingDto?> GetByKeyAsync(string key)
    {
        var setting = await _settings.Query()
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null) return null;

        return new SettingDto
        {
            Id = setting.Id,
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description
        };
    }

    public async Task UpdateAsync(int id, UpdateSettingDto dto)
    {
        var setting = await _settings.GetByIdAsync(id);
        if (setting == null)
            throw new KeyNotFoundException("Setting not found");

        setting.Value = dto.Value;
        setting.UpdatedDate = DateTime.UtcNow;

        await _settings.UpdateAsync(setting);
        await _unitOfWork.SaveChangesAsync();
    }
}
