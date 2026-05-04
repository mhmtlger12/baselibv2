using Baselib.Entities;

namespace Baselib.Business.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<IEnumerable<Department>> GetAllWithSubDepartmentsAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<Department?> GetByCodeAsync(string code);
    Task<Department> AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(int id);
}