using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IDepartmentService
{
    Task<IDataResult<IEnumerable<DepartmentDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<SelectOptionDto>>> GetSelectOptionsAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<DepartmentDto>>> GetTreeAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<DepartmentDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<DepartmentDto>> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(int id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}