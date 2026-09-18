using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IMenuService
{
    Task<IDataResult<IEnumerable<MenuDto>>> GetAllAsync();
    Task<IDataResult<IEnumerable<MenuDto>>> GetMenusForUserAsync(int userId, int? activeRoleId);
    Task<IDataResult<MenuDto>> GetByIdAsync(int id);
    Task<IDataResult<MenuDto>> CreateAsync(CreateMenuDto dto);
    Task<IResult> UpdateAsync(int id, UpdateMenuDto dto);
    Task<IResult> DeleteAsync(int id);
}
