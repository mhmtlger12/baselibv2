using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IPermissionService
{
    Task<IDataResult<IEnumerable<PermissionDto>>> GetAllAsync();
    Task<IDataResult<PermissionDto>> GetByIdAsync(int id);
    Task<IDataResult<PermissionDto>> CreateAsync(CreatePermissionDto dto);
    Task<IResult> UpdateAsync(int id, CreatePermissionDto dto);
    Task<IResult> DeleteAsync(int id);
    Task<IDataResult<IEnumerable<PermissionGroupDto>>> GetGroupedPermissionsAsync(int? roleId = null);
    Task<IResult> SaveRolePermissionsAsync(int roleId, List<PermissionGroupDto> permissionGroups);
    Task<List<int>> ResolvePermissionIdsAsync(List<PermissionGroupDto> permissionGroups);
}