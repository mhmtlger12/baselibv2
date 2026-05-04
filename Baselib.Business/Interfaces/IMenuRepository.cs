using Baselib.Entities;

namespace Baselib.Business.Interfaces;

public interface IMenuRepository
{
    Task<IEnumerable<Menu>> GetAllWithSubMenusAsync();
    Task<IEnumerable<Menu>> GetMenusByUserIdAsync(int userId);
    Task<Menu?> GetByIdAsync(int id);
    Task<Menu> AddAsync(Menu menu);
    Task UpdateAsync(Menu menu);
    Task DeleteAsync(int id);
}