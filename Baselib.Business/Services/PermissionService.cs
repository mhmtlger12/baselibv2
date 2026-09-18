using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class PermissionService : IPermissionService
{
    private readonly IRepository<Permission> _permissions;
    private readonly IRepository<RolePermission> _rolePermissions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public PermissionService(
        IRepository<Permission> permissions,
        IRepository<RolePermission> rolePermissions,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _permissions = permissions;
        _rolePermissions = rolePermissions;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<PermissionDto>>> GetAllAsync()
    {
        var permissions = await _permissions.GetAllAsync(asNoTracking: true);
        var orderedPermissions = permissions
            .OrderBy(p => p.ControllerName)
            .ThenBy(p => p.CRUDActionType);

        return DataResult<IEnumerable<PermissionDto>>.Ok(_mapper.Map<IEnumerable<PermissionDto>>(orderedPermissions));
    }

    public async Task<IDataResult<PermissionDto>> GetByIdAsync(int id)
    {
        var permission = await _permissions.GetByIdAsync(id);
        if (permission == null)
            return DataResult<PermissionDto>.NotFound(Messages.Permission.NotFound);

        return DataResult<PermissionDto>.Ok(_mapper.Map<PermissionDto>(permission));
    }

    public async Task<IDataResult<PermissionDto>> CreateAsync(CreatePermissionDto dto)
    {
        var permission = BuildPermission(dto);

        if (await _permissions.AnyAsync(p => p.Code == permission.Code, ignoreQueryFilters: true))
            return DataResult<PermissionDto>.BadRequest(Messages.Permission.CodeAlreadyExists);

        if (await _permissions.AnyAsync(p =>
                p.ControllerName == permission.ControllerName &&
                p.ActionName == permission.ActionName,
                ignoreQueryFilters: true))
            return DataResult<PermissionDto>.BadRequest(Messages.Permission.AlreadyExistsForAction);

        await _permissions.AddAsync(permission);
        await _unitOfWork.SaveChangesAsync();

        return DataResult<PermissionDto>.Created(_mapper.Map<PermissionDto>(permission), Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, CreatePermissionDto dto)
    {
        var permission = await _permissions.GetByIdAsync(id);
        if (permission == null)
            return Result.NotFound(Messages.Permission.NotFound);

        var normalized = BuildPermission(dto);

        if (await _permissions.AnyAsync(
                p => p.Code == normalized.Code && p.Id != id,
                ignoreQueryFilters: true))
            return Result.BadRequest(Messages.Permission.CodeAlreadyExists);

        if (await _permissions.AnyAsync(p =>
                p.Id != id &&
                p.ControllerName == normalized.ControllerName &&
                p.ActionName == normalized.ActionName,
                ignoreQueryFilters: true))
            return Result.BadRequest(Messages.Permission.AlreadyExistsForAction);

        permission.Name = normalized.Name;
        permission.Code = normalized.Code;
        permission.Description = normalized.Description;
        permission.ControllerName = normalized.ControllerName;
        permission.ActionName = normalized.ActionName;
        permission.CRUDActionType = normalized.CRUDActionType;
        permission.IsActive = dto.IsActive;
        permission.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

        _permissions.Update(permission);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var permission = await _permissions.GetByIdAsync(id);
        if (permission == null)
            return Result.NotFound(Messages.Permission.NotFound);

        permission.IsActive = false;
        permission.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;
        _permissions.Update(permission);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Deleted);
    }

    public async Task<IDataResult<IEnumerable<PermissionGroupDto>>> GetGroupedPermissionsAsync(int? roleId = null)
    {
        var allPermissions = await _permissions.GetAllAsync();
        var orderedPermissions = allPermissions
            .OrderBy(p => p.ControllerName)
            .ThenBy(p => p.CRUDActionType);

        var rolePermissionIds = roleId.HasValue
            ? (await _rolePermissions.GetAllAsync(rp => rp.RoleId == roleId.Value))
                .Select(rp => rp.PermissionId)
                .ToList()
            : new List<int>();

        var groups = PermissionGroupHelper.BuildGroups(orderedPermissions, rolePermissionIds);
        return DataResult<IEnumerable<PermissionGroupDto>>.Ok(groups);
    }

    private Permission BuildPermission(CreatePermissionDto dto)
    {
        var controller = dto.ControllerName?.Trim() ?? string.Empty;
        var action = dto.ActionName?.Trim() ?? string.Empty;
        var code = dto.Code.Trim();

        return new Permission
        {
            Name = string.IsNullOrWhiteSpace(dto.Name) ? $"{controller} {action}" : dto.Name.Trim(),
            Code = code,
            Description = dto.Description?.Trim(),
            ControllerName = controller,
            ActionName = action,
            CRUDActionType = dto.CRUDActionType,
            CreatedDate = _timeProvider.GetUtcNow().UtcDateTime,
            IsActive = dto.IsActive
        };
    }
}
