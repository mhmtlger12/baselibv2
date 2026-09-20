using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IJobListingService
{
    Task<IDataResult<IEnumerable<JobListingDto>>> GetAllAsync(CancellationToken ct = default);
    Task<IDataResult<IEnumerable<JobListingDto>>> GetPublishedAsync(string? categoryKey = null, string? query = null, CancellationToken ct = default);
    Task<IDataResult<JobListingDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IDataResult<JobListingDto>> CreateAsync(SaveJobListingDto dto, CancellationToken ct = default);
    Task<IResult> UpdateAsync(int id, SaveJobListingDto dto, CancellationToken ct = default);
    Task<IResult> DeleteAsync(int id, CancellationToken ct = default);
}
