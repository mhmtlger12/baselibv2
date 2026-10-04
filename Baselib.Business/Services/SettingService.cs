using Baselib.Core.Constants;
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
    private readonly IEntityRepository<AppSetting> _settings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public SettingService(IEntityRepository<AppSetting> settings, IUnitOfWork unitOfWork, IMapper mapper, TimeProvider timeProvider)
    {
        _settings = settings;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<SettingDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _settings.GetAllAsync(asNoTracking: true, cancellationToken: cancellationToken);
        return DataResult<IEnumerable<SettingDto>>.Ok(_mapper.Map<IEnumerable<SettingDto>>(settings.OrderBy(s => s.Key)));
    }

    public async Task<IDataResult<PublicSiteSettingsDto>> GetPublicAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _settings.GetAllAsync(
            predicate: setting => setting.Key == SiteSettingConstants.Name || setting.Key == SiteSettingConstants.Tagline,
            asNoTracking: true, cancellationToken: cancellationToken, includes: []);
        var values = settings.ToList();
        var name = values.FirstOrDefault(setting => setting.Key == SiteSettingConstants.Name)?.Value.Trim();
        var tagline = values.FirstOrDefault(setting => setting.Key == SiteSettingConstants.Tagline)?.Value.Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(tagline))
            return DataResult<PublicSiteSettingsDto>.ErrorDataResult("Site bilgileri henüz yapılandırılmadı.", 503);

        return DataResult<PublicSiteSettingsDto>.Ok(new PublicSiteSettingsDto { Name = name, Tagline = tagline });
    }

    public async Task<IResult> UpdateAsync(int id, UpdateSettingDto dto, CancellationToken cancellationToken = default)
    {
        var setting = await _settings.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (setting == null)
            return Result.NotFound(Messages.Settings.NotFound);

        var maxLength = setting.Key switch
        {
            SiteSettingConstants.Name => SiteSettingConstants.NameMaxLength,
            SiteSettingConstants.Tagline => SiteSettingConstants.TaglineMaxLength,
            _ => (int?)null
        };
        if (maxLength.HasValue && (string.IsNullOrWhiteSpace(dto.Value) || dto.Value.Trim().Length > maxLength.Value))
            return Result.BadRequest($"Bu ayar boş bırakılamaz ve en fazla {maxLength.Value} karakter olabilir.");

        setting.Value = maxLength.HasValue ? dto.Value.Trim() : dto.Value;
        setting.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

        _settings.Update(setting);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

        return Result.Ok(Messages.General.Updated);
    }
}
