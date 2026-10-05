using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IJobListingService
{
    Task<IDataResult<IReadOnlyList<JobCategoryDto>>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<JobListingDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<JobListingDto>>> GetPublishedAsync(string? categoryKey = null, string? query = null, CancellationToken cancellationToken = default);
    Task<IDataResult<JobListingDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<JobListingDto>> GetPublishedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<JobListingDto>> CreateAsync(SaveJobListingDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(int id, SaveJobListingDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
