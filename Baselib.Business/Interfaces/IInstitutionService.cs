using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IInstitutionService
{
    Task<IDataResult<IEnumerable<InstitutionDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<InstitutionDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<InstitutionDto>> CreateAsync(SaveInstitutionDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(int id, SaveInstitutionDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
