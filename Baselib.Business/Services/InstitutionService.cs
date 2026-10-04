using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class InstitutionService(
    IEntityRepository<Institution> institutions,
    IUnitOfWork unitOfWork,
    AutoMapper.IMapper mapper,
    TimeProvider timeProvider) : IInstitutionService
{
    public async Task<IDataResult<IEnumerable<InstitutionDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await institutions.GetAllAsync(predicate: institution => !institution.IsDeleted,
            ignoreQueryFilters: true, asNoTracking: true, cancellationToken: cancellationToken, includes: []);
        return DataResult<IEnumerable<InstitutionDto>>.Ok(items.OrderBy(x => x.Name).Select(mapper.Map<InstitutionDto>));
    }

    public async Task<IDataResult<InstitutionDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await institutions.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted,
            ignoreQueryFilters: true, includes: [], cancellationToken: cancellationToken);
        return item is null ? DataResult<InstitutionDto>.NotFound("Kurum bulunamadı.") : DataResult<InstitutionDto>.Ok(mapper.Map<InstitutionDto>(item));
    }

    public async Task<IDataResult<InstitutionDto>> CreateAsync(SaveInstitutionDto dto, CancellationToken cancellationToken = default)
    {
        var item = new Institution { CreatedDate = timeProvider.GetUtcNow().UtcDateTime };
        Apply(item, dto);
        await institutions.AddAsync(item, cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return DataResult<InstitutionDto>.Created(mapper.Map<InstitutionDto>(item), Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, SaveInstitutionDto dto, CancellationToken cancellationToken = default)
    {
        var item = await institutions.FirstOrDefaultAsync(x => x.Id == id, ignoreQueryFilters: true, includes: [], cancellationToken: cancellationToken);
        if (item is null || item.IsDeleted) return Result.NotFound("Kurum bulunamadı.");
        Apply(item, dto);
        item.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        institutions.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await institutions.FirstOrDefaultAsync(x => x.Id == id, ignoreQueryFilters: true, includes: [], cancellationToken: cancellationToken);
        if (item is null || item.IsDeleted) return Result.NotFound("Kurum bulunamadı.");
        item.IsDeleted = true;
        item.IsActive = false;
        item.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        institutions.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return Result.Ok(Messages.General.Deleted);
    }

    private static void Apply(Institution item, SaveInstitutionDto dto)
    {
        item.Name = dto.Name.Trim();
        item.LogoUrl = string.IsNullOrWhiteSpace(dto.LogoUrl) ? null : dto.LogoUrl.Trim();
        item.WebsiteUrl = string.IsNullOrWhiteSpace(dto.WebsiteUrl) ? null : dto.WebsiteUrl.Trim();
        item.IsActive = dto.IsActive;
    }
}
