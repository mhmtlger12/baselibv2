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

    public PermissionService(
        IRepository<Permission> permissions,
        IRepository<RolePermission> rolePermissions,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _permissions = permissions;
        _rolePermissions = rolePermissions;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<PermissionDto>>> GetAllAsync()
    {
        var permissions = await _permissions.GetAllAsync();
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

        if (await _permissions.AnyAsync(p => p.Code == permission.Code))
            return DataResult<PermissionDto>.BadRequest(Messages.Permission.CodeAlreadyExists);

        if (await _permissions.AnyAsync(p =>
                p.ControllerName == permission.ControllerName &&
                p.ActionName == permission.ActionName))
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

        if (await _permissions.AnyAsync(p => p.Code == normalized.Code && p.Id != id))
            return Result.BadRequest(Messages.Permission.CodeAlreadyExists);

        if (await _permissions.AnyAsync(p =>
                p.Id != id &&
                p.ControllerName == normalized.ControllerName &&
                p.ActionName == normalized.ActionName))
            return Result.BadRequest(Messages.Permission.AlreadyExistsForAction);

        permission.Name = normalized.Name;
        permission.Code = normalized.Code;
        permission.Description = normalized.Description;
        permission.ControllerName = normalized.ControllerName;
        permission.ActionName = normalized.ActionName;
        permission.CRUDActionType = normalized.CRUDActionType;
        permission.IsActive = dto.IsActive;
        permission.UpdatedDate = DateTime.UtcNow;

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
        permission.UpdatedDate = DateTime.UtcNow;
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

    public async Task<IResult> SaveRolePermissionsAsync(int roleId, List<PermissionGroupDto> permissionGroups)
    {
        var existingRolePermissions = await _rolePermissions.GetAllAsync(rp => rp.RoleId == roleId);
        _rolePermissions.RemoveRange(existingRolePermissions);

        var validPermissionIds = await ResolvePermissionIdsAsync(permissionGroups);

        var rolePermissions = validPermissionIds
            .Select(permissionId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            })
            .ToList();

        if (rolePermissions.Count > 0)
            await _rolePermissions.AddRangeAsync(rolePermissions);

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Saved);
    }

    public async Task<List<int>> ResolvePermissionIdsAsync(List<PermissionGroupDto> permissionGroups)
    {
        var selectedPermissionIds = permissionGroups
            .SelectMany(group => group.ControllerCrudList)
            .Where(crud => crud.Checked && crud.PermissionId > 0)
            .Select(crud => crud.PermissionId)
            .Distinct()
            .ToList();

        if (!selectedPermissionIds.Any())
        {
            foreach (var group in permissionGroups.Where(p => p.Checked || p.ControllerCrudList.Any(c => c.Checked)))
            {
                var crudTypes = group.ControllerCrudList
                    .Where(c => c.Checked)
                    .Select(c => c.CRUDActionType)
                    .ToList();

                var fallbackPermissions = await _permissions.GetAllAsync(
                    p => p.ControllerName == group.ControllerName && crudTypes.Contains(p.CRUDActionType));

                selectedPermissionIds.AddRange(fallbackPermissions.Select(p => p.Id));
            }
        }

        var distinctSelectedPermissionIds = selectedPermissionIds.Distinct().ToList();

        var validPermissions = await _permissions.GetAllAsync(
            p => distinctSelectedPermissionIds.Contains(p.Id));
        return validPermissions.Select(p => p.Id).ToList();
    }

    private static Permission BuildPermission(CreatePermissionDto dto)
    {
        var controller = dto.ControllerName?.Trim() ?? string.Empty;
        var action = dto.ActionName?.Trim() ?? string.Empty;
        var code = string.IsNullOrWhiteSpace(dto.Code)
            ? $"{controller}_{action}"
            : dto.Code.Trim();

        return new Permission
        {
            Name = string.IsNullOrWhiteSpace(dto.Name) ? $"{controller} {action}" : dto.Name.Trim(),
            Code = code,
            Description = dto.Description?.Trim(),
            ControllerName = controller,
            ActionName = action,
            CRUDActionType = dto.CRUDActionType,
            CreatedDate = DateTime.UtcNow,
            IsActive = dto.IsActive
        };
    }
}
