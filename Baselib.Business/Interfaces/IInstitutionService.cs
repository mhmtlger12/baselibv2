using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IInstitutionService
{
    Task<IDataResult<IEnumerable<InstitutionDto>>> GetAllAsync(CancellationToken ct = default);
    Task<IDataResult<InstitutionDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IDataResult<InstitutionDto>> CreateAsync(SaveInstitutionDto dto, CancellationToken ct = default);
    Task<IResult> UpdateAsync(int id, SaveInstitutionDto dto, CancellationToken ct = default);
    Task<IResult> DeleteAsync(int id, CancellationToken ct = default);
}
