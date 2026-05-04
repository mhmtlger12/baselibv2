using Baselib.Entities;

namespace Baselib.Business.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<Permission?> GetByIdAsync(int id);
    Task<Permission?> GetByCodeAsync(string code);
    Task<Permission> AddAsync(Permission permission);
    Task UpdateAsync(Permission permission);
    Task DeleteAsync(int id);
}