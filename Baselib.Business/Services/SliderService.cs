using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class SliderService(
    IRepository<Slider> sliders,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    TimeProvider timeProvider) : ISliderService
{
    public async Task<IDataResult<IEnumerable<SliderDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await sliders.GetAllAsync(predicate: slider => !slider.IsDeleted,
            ignoreQueryFilters: true, asNoTracking: true, cancellationToken: ct, includes: []);
        return MapList(items);
    }

    public async Task<IDataResult<IEnumerable<SliderDto>>> GetPublishedAsync(CancellationToken ct = default)
    {
        var items = await sliders.GetAllAsync(asNoTracking: true, cancellationToken: ct);
        return MapList(items);
    }

    public async Task<IDataResult<SliderDto>> GetByIdAsync(int id)
    {
        var slider = await FindAsync(id);
        return slider is null
            ? DataResult<SliderDto>.NotFound(Messages.Slider.NotFound)
            : DataResult<SliderDto>.Ok(mapper.Map<SliderDto>(slider));
    }

    public async Task<IDataResult<SliderDto>> CreateAsync(SaveSliderDto dto)
    {
        var slider = new Slider { CreatedDate = timeProvider.GetUtcNow().UtcDateTime };
        Apply(slider, dto);
        await sliders.AddAsync(slider);
        await unitOfWork.SaveChangesAsync();
        return DataResult<SliderDto>.Created(mapper.Map<SliderDto>(slider), Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, SaveSliderDto dto)
    {
        var slider = await FindAsync(id);
        if (slider is null) return Result.NotFound(Messages.Slider.NotFound);
        Apply(slider, dto);
        slider.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        sliders.Update(slider);
        await unitOfWork.SaveChangesAsync();
        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var slider = await FindAsync(id);
        if (slider is null) return Result.NotFound(Messages.Slider.NotFound);
        slider.IsDeleted = true;
        slider.IsActive = false;
        slider.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        sliders.Update(slider);
        await unitOfWork.SaveChangesAsync();
        return Result.Ok(Messages.General.Deleted);
    }

    private Task<Slider?> FindAsync(int id) => sliders.FirstOrDefaultAsync(
        slider => slider.Id == id && !slider.IsDeleted, ignoreQueryFilters: true, includes: []);

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
