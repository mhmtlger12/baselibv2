using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class SliderService(
    IEntityRepository<Slider> sliders,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    TimeProvider timeProvider) : ISliderService
{
    public async Task<IDataResult<IEnumerable<SliderDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await sliders.GetAllAsync(predicate: slider => !slider.IsDeleted,
            ignoreQueryFilters: true, asNoTracking: true, cancellationToken: cancellationToken, includes: []);
        return MapList(items);
    }

    public async Task<IDataResult<IEnumerable<SliderDto>>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var items = await sliders.GetAllAsync(asNoTracking: true, cancellationToken: cancellationToken);
        return MapList(items);
    }

    public async Task<IDataResult<SliderDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var slider = await FindAsync(id, cancellationToken: cancellationToken);
        return slider is null
            ? DataResult<SliderDto>.NotFound(Messages.Slider.NotFound)
            : DataResult<SliderDto>.Ok(mapper.Map<SliderDto>(slider));
    }

    public async Task<IDataResult<SliderDto>> CreateAsync(SaveSliderDto dto, CancellationToken cancellationToken = default)
    {
        var slider = new Slider { CreatedDate = timeProvider.GetUtcNow().UtcDateTime };
        Apply(slider, dto);
        await sliders.AddAsync(slider, cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return DataResult<SliderDto>.Created(mapper.Map<SliderDto>(slider), Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, SaveSliderDto dto, CancellationToken cancellationToken = default)
    {
        var slider = await FindAsync(id, cancellationToken: cancellationToken);
        if (slider is null) return Result.NotFound(Messages.Slider.NotFound);
        Apply(slider, dto);
        slider.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        sliders.Update(slider);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var slider = await FindAsync(id, cancellationToken: cancellationToken);
        if (slider is null) return Result.NotFound(Messages.Slider.NotFound);
        slider.IsDeleted = true;
        slider.IsActive = false;
        slider.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        sliders.Update(slider);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return Result.Ok(Messages.General.Deleted);
    }

    private Task<Slider?> FindAsync(int id, CancellationToken cancellationToken = default) => sliders.FirstOrDefaultAsync(
        slider => slider.Id == id && !slider.IsDeleted, ignoreQueryFilters: true, includes: [], cancellationToken: cancellationToken);

    private IDataResult<IEnumerable<SliderDto>> MapList(IEnumerable<Slider> items) =>
        DataResult<IEnumerable<SliderDto>>.Ok(items.OrderBy(slider => slider.Order)
            .ThenBy(slider => slider.Id).Select(slider => mapper.Map<SliderDto>(slider)).ToList());

    private static void Apply(Slider slider, SaveSliderDto dto)
    {
        slider.Title = dto.Title.Trim();
        slider.Description = dto.Description?.Trim() ?? string.Empty;
        slider.ImageUrl = dto.ImageUrl.Trim();
        slider.LinkUrl = string.IsNullOrWhiteSpace(dto.LinkUrl) ? null : dto.LinkUrl.Trim();
        slider.Order = dto.Order;
        slider.IsActive = dto.IsActive;
    }
}
