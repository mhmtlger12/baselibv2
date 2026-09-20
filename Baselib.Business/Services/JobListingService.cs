using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class JobListingService(
    IRepository<JobListing> listings,
    IRepository<Institution> institutions,
    IUnitOfWork unitOfWork,
    AutoMapper.IMapper mapper,
    TimeProvider timeProvider) : IJobListingService
{
    public async Task<IDataResult<IEnumerable<JobListingDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await listings.GetAllAsync(predicate: job => !job.IsDeleted, ignoreQueryFilters: true,
            asNoTracking: true, cancellationToken: ct, includes: [job => job.InstitutionEntity!]);
        return DataResult<IEnumerable<JobListingDto>>.Ok(MapList(items));
    }

    public async Task<IDataResult<IEnumerable<JobListingDto>>> GetPublishedAsync(string? categoryKey = null, string? query = null, CancellationToken ct = default)
    {
        var items = await listings.GetAllAsync(predicate: job =>
                (string.IsNullOrWhiteSpace(categoryKey) || categoryKey == "all" || job.CategoryKey == categoryKey) &&
                (string.IsNullOrWhiteSpace(query) || job.Institution.Contains(query) || job.Summary.Contains(query)),
            asNoTracking: true, cancellationToken: ct, includes: [job => job.InstitutionEntity!]);
        return DataResult<IEnumerable<JobListingDto>>.Ok(MapList(items));
    }

    public async Task<IDataResult<JobListingDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await listings.FirstOrDefaultAsync(job => job.Id == id, includes: [job => job.InstitutionEntity!]);
        return item is null ? DataResult<JobListingDto>.NotFound("İlan bulunamadı.") : DataResult<JobListingDto>.Ok(mapper.Map<JobListingDto>(item));
    }

    public async Task<IDataResult<JobListingDto>> CreateAsync(SaveJobListingDto dto, CancellationToken ct = default)
    {
        var item = new JobListing { CreatedDate = timeProvider.GetUtcNow().UtcDateTime };
        await ApplyAsync(item, dto);
        await listings.AddAsync(item);
        await unitOfWork.SaveChangesAsync();
        return DataResult<JobListingDto>.Created(mapper.Map<JobListingDto>(item), Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, SaveJobListingDto dto, CancellationToken ct = default)
    {
        var item = await listings.FirstOrDefaultAsync(job => job.Id == id, ignoreQueryFilters: true, includes: []);
        if (item is null || item.IsDeleted) return Result.NotFound("İlan bulunamadı.");
        await ApplyAsync(item, dto);
        item.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        listings.Update(item);
        await unitOfWork.SaveChangesAsync();
        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        var item = await listings.FirstOrDefaultAsync(job => job.Id == id, ignoreQueryFilters: true, includes: []);
        if (item is null || item.IsDeleted) return Result.NotFound("İlan bulunamadı.");
        item.IsDeleted = true;
        item.IsActive = false;
        item.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        listings.Update(item);
        await unitOfWork.SaveChangesAsync();
        return Result.Ok(Messages.General.Deleted);
    }

    private IEnumerable<JobListingDto> MapList(IEnumerable<JobListing> items) => items.OrderByDescending(x => x.PublishedAt).ThenBy(x => x.Id).Select(item =>
    {
        var dto = mapper.Map<JobListingDto>(item);
        dto.InstitutionId = item.InstitutionId;
        dto.InstitutionLogoUrl = item.InstitutionEntity?.LogoUrl;
        return dto;
    });

    private async Task ApplyAsync(JobListing item, SaveJobListingDto dto)
    {
        item.InstitutionId = dto.InstitutionId;
        if (dto.InstitutionId is int institutionId)
        {
            var institution = await institutions.FirstOrDefaultAsync(x => x.Id == institutionId);
            if (institution is not null) item.Institution = institution.Name;
        }
        else item.Institution = dto.Institution.Trim();
        item.Summary = dto.Summary.Trim();
        item.CategoryKey = dto.CategoryKey.Trim();
        item.CategoryLabel = dto.CategoryLabel.Trim();
        item.PublishedAt = dto.PublishedAt;
        item.StartDate = dto.StartDate.Trim();
        item.EndDate = dto.EndDate.Trim();
        item.SourceUrl = string.IsNullOrWhiteSpace(dto.SourceUrl) ? null : dto.SourceUrl.Trim();
        item.PdfUrl = string.IsNullOrWhiteSpace(dto.PdfUrl) ? null : dto.PdfUrl.Trim();
        item.IsActive = dto.IsActive;
    }
}
