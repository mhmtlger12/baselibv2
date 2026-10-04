using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IMenuService
{
    Task<IDataResult<IEnumerable<MenuDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<MenuDto>>> GetMenusForUserAsync(int userId, int? activeRoleId, CancellationToken cancellationToken = default);
    Task<IDataResult<MenuDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<MenuDto>> CreateAsync(CreateMenuDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(int id, UpdateMenuDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
