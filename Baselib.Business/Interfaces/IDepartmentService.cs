using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IDepartmentService
{
    Task<IDataResult<IEnumerable<DepartmentDto>>> GetAllAsync();
    Task<IDataResult<IEnumerable<DepartmentDto>>> GetTreeAsync();
    Task<IDataResult<DepartmentDto>> GetByIdAsync(int id);
    Task<IDataResult<DepartmentDto>> CreateAsync(CreateDepartmentDto dto);
    Task<IResult> UpdateAsync(int id, UpdateDepartmentDto dto);
    Task<IResult> DeleteAsync(int id);
}