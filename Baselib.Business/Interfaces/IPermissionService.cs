using Baselib.Business.DTOs;
using Baselib.Core.Results;

namespace Baselib.Business.Interfaces;

public interface IPermissionService
{
    Task<IDataResult<IEnumerable<PermissionDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<PermissionDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<PermissionDto>> CreateAsync(CreatePermissionDto dto, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(int id, CreatePermissionDto dto, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IDataResult<IEnumerable<PermissionGroupDto>>> GetGroupedPermissionsAsync(int? roleId = null, CancellationToken cancellationToken = default);
}
