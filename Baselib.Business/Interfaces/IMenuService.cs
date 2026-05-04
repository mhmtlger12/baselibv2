using Baselib.Business.DTOs;

namespace Baselib.Business.Interfaces;

public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetAllAsync();
    Task<IEnumerable<MenuDto>> GetMenusByUserIdAsync(int userId);
    Task<MenuDto?> GetByIdAsync(int id);
    Task<MenuDto> CreateAsync(CreateMenuDto dto);
    Task UpdateAsync(int id, UpdateMenuDto dto);
    Task DeleteAsync(int id);
}