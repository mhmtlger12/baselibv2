using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IUserService
{
    Task<IDataResult<IEnumerable<UserDto>>> GetAllAsync();
    Task<IDataResult<UserDto>> GetByIdAsync(int id, int? activeRoleId = null);
    Task<IDataResult<UserDto>> CreateAsync(CreateUserDto dto);
    Task<IResult> UpdateAsync(int id, UpdateUserDto dto);
    Task<IResult> DeleteAsync(int id);
    Task<IResult> AssignRolesAsync(int userId, List<int> roleIds);
    Task<IResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}