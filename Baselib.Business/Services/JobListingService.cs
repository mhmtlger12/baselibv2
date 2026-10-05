using System.ComponentModel.DataAnnotations;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class JobListingService(
    IEntityRepository<JobListing> listings,
    IEntityRepository<Institution> institutions,
    IEntityRepository<JobCategory> categories,
    IUnitOfWork unitOfWork,
    AutoMapper.IMapper mapper,
    TimeProvider timeProvider) : IJobListingService
{
    public async Task<IDataResult<IReadOnlyList<JobCategoryDto>>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var items = (await categories.GetAllAsync(asNoTracking: true, cancellationToken: cancellationToken)).ToList();
        // Only categories reachable from an active root are usable (also excludes cycles/orphans).
        var enabled = new List<JobCategoryDto>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        void AddChildren(string? parentKey)
        {
            foreach (var category in items.Where(x => x.ParentKey == parentKey).OrderBy(x => x.SortOrder).ThenBy(x => x.Id))
            {
                if (!visited.Add(category.Key)) continue;
                enabled.Add(new(category.Key, category.Label, category.ParentKey, category.SortOrder, category.IsSelectable));
                AddChildren(category.Key);
            }
        }
        AddChildren(null);
        return DataResult<IReadOnlyList<JobCategoryDto>>.Ok(enabled);
    }

    public async Task<IDataResult<IEnumerable<JobListingDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await listings.GetAllAsync(predicate: job => !job.IsDeleted, ignoreQueryFilters: true,
            asNoTracking: true, cancellationToken: cancellationToken, includes: [job => job.InstitutionEntity!]);
        return DataResult<IEnumerable<JobListingDto>>.Ok(MapList(items));
    }

    public async Task<IDataResult<IEnumerable<JobListingDto>>> GetPublishedAsync(string? categoryKey = null, string? query = null, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var items = await listings.GetAllAsync(predicate: job =>
                job.IsActive && !job.IsDeleted && job.PublishedAt <= now &&
                (string.IsNullOrWhiteSpace(categoryKey) || categoryKey == "all" || job.CategoryKey == categoryKey) &&
                (string.IsNullOrWhiteSpace(query) || (job.InstitutionEntity != null ? job.InstitutionEntity.Name : job.Institution).Contains(query) || job.Summary.Contains(query)),
            ignoreQueryFilters: true, asNoTracking: true, cancellationToken: cancellationToken, includes: [job => job.InstitutionEntity!]);
        return DataResult<IEnumerable<JobListingDto>>.Ok(MapList(items));
    }

    public async Task<IDataResult<JobListingDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await listings.FirstOrDefaultAsync(job => job.Id == id && !job.IsDeleted,
            ignoreQueryFilters: true, includes: [job => job.InstitutionEntity!], cancellationToken: cancellationToken);
        return item is null ? DataResult<JobListingDto>.NotFound("İlan bulunamadı.") : DataResult<JobListingDto>.Ok(mapper.Map<JobListingDto>(item));
    }

    public async Task<IDataResult<JobListingDto>> GetPublishedByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var item = await listings.FirstOrDefaultAsync(job => job.Id == id && job.IsActive && !job.IsDeleted && job.PublishedAt <= now,
            ignoreQueryFilters: true, includes: [job => job.InstitutionEntity!], cancellationToken: cancellationToken);
        return item is null ? DataResult<JobListingDto>.NotFound("İlan bulunamadı.") : DataResult<JobListingDto>.Ok(mapper.Map<JobListingDto>(item));
    }

    public async Task<IDataResult<JobListingDto>> CreateAsync(SaveJobListingDto dto, CancellationToken cancellationToken = default)
    {
        var item = new JobListing { CreatedDate = timeProvider.GetUtcNow().UtcDateTime };
        var validation = await ApplyAsync(item, dto, cancellationToken);
        if (!validation.Success) return DataResult<JobListingDto>.BadRequest(validation.Message);
        await listings.AddAsync(item, cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return DataResult<JobListingDto>.Created(mapper.Map<JobListingDto>(item), Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, SaveJobListingDto dto, CancellationToken cancellationToken = default)
    {
        var item = await listings.FirstOrDefaultAsync(job => job.Id == id, ignoreQueryFilters: true, includes: [], cancellationToken: cancellationToken);
        if (item is null || item.IsDeleted) return Result.NotFound("İlan bulunamadı.");
        var validation = await ApplyAsync(item, dto, cancellationToken);
        if (!validation.Success) return validation;
        item.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        listings.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await listings.FirstOrDefaultAsync(job => job.Id == id, ignoreQueryFilters: true, includes: [], cancellationToken: cancellationToken);
        if (item is null || item.IsDeleted) return Result.NotFound("İlan bulunamadı.");
        item.IsDeleted = true;
        item.IsActive = false;
        item.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        listings.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return Result.Ok(Messages.General.Deleted);
    }

    private IEnumerable<JobListingDto> MapList(IEnumerable<JobListing> items) =>
        items.OrderByDescending(x => x.PublishedAt).ThenBy(x => x.Id).Select(mapper.Map<JobListingDto>);

    private async Task<IResult> ApplyAsync(JobListing item, SaveJobListingDto dto, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationResult>();
        if (!Validator.TryValidateObject(dto, new ValidationContext(dto), errors, validateAllProperties: true))
            return Result.BadRequest(string.Join(" ", errors.Select(error => error.ErrorMessage)));

        Institution? institution = null;
        if (dto.InstitutionId is int institutionId)
        {
            institution = await institutions.FirstOrDefaultAsync(x => x.Id == institutionId && x.IsActive && !x.IsDeleted,
                cancellationToken: cancellationToken);
            if (institution is null) return Result.BadRequest("Lütfen geçerli ve aktif bir kurum seçin.");
        }
        else if (item.Id == 0 || item.InstitutionId is not null || string.IsNullOrWhiteSpace(item.Institution))
            return Result.BadRequest("Lütfen geçerli ve aktif bir kurum seçin.");

        var availableCategories = await GetCategoriesAsync(cancellationToken);
        var category = availableCategories.Data.FirstOrDefault(x => x.Key == dto.CategoryKey.Trim() && x.IsSelectable);
        if (category is null) return Result.BadRequest("Lütfen geçerli ve aktif bir ilan kategorisi seçin.");

        // Do not mutate the tracked listing until every business rule has passed.
        item.InstitutionId = dto.InstitutionId;
        item.InstitutionEntity = institution;
        if (institution is not null) item.Institution = institution.Name;
        // Existing unlinked records keep their original name until an institution is selected.
        item.Summary = dto.Summary.Trim();
        item.CategoryKey = category.Key;
        item.CategoryLabel = category.Label;
        item.PublishedAt = dto.PublishedAt.Kind == DateTimeKind.Local
            ? dto.PublishedAt.ToUniversalTime() : DateTime.SpecifyKind(dto.PublishedAt, DateTimeKind.Utc);
        item.StartDate = dto.StartDate.Trim();
        item.EndDate = dto.EndDate.Trim();
        item.SourceUrl = string.IsNullOrWhiteSpace(dto.SourceUrl) ? null : dto.SourceUrl.Trim();
        item.PdfUrl = string.IsNullOrWhiteSpace(dto.PdfUrl) ? null : dto.PdfUrl.Trim();
        item.IsActive = dto.IsActive;
        return Result.Ok();
    }
}
