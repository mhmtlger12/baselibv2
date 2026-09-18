using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class SettingService : ISettingService
{
    private readonly IRepository<AppSetting> _settings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public SettingService(IRepository<AppSetting> settings, IUnitOfWork unitOfWork, IMapper mapper, TimeProvider timeProvider)
    {
        _settings = settings;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<SettingDto>>> GetAllAsync()
    {
        var settings = await _settings.GetAllAsync(asNoTracking: true);
        return DataResult<IEnumerable<SettingDto>>.Ok(_mapper.Map<IEnumerable<SettingDto>>(settings.OrderBy(s => s.Key)));
    }

    public async Task<IResult> UpdateAsync(int id, UpdateSettingDto dto)
    {
        var setting = await _settings.GetByIdAsync(id);
        if (setting == null)
            return Result.NotFound(Messages.Settings.NotFound);

        setting.Value = dto.Value;
        setting.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

        _settings.Update(setting);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }
}
