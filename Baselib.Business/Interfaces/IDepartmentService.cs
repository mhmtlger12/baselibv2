using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<IEnumerable<DepartmentDto>> GetTreeAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
    Task UpdateAsync(int id, UpdateDepartmentDto dto);
    Task DeleteAsync(int id);
}