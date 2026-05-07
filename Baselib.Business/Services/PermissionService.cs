using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var permissions = await _permissions.Query()
            .Where(p => p.IsActive)
            .OrderBy(p => p.ControllerName)
            .ThenBy(p => p.CRUDActionType)
            .ToListAsync();

        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<PermissionDto?> GetByIdAsync(int id)
    {
        var permission = await _permissions.GetByIdAsync(id);
        return permission == null ? null : _mapper.Map<PermissionDto>(permission);
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
    {
        var permission = BuildPermission(dto);

        if (await _permissions.AnyAsync(p => p.Code == permission.Code))
            throw new InvalidOperationException(Messages.Permission.CodeAlreadyExists);

        if (await _permissions.AnyAsync(p =>
                p.ControllerName == permission.ControllerName &&
                p.ActionName == permission.ActionName))
            throw new InvalidOperationException(Messages.Permission.AlreadyExistsForAction);

        await _permissions.AddAsync(permission);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PermissionDto>(permission);
    }

    public async Task UpdateAsync(int id, CreatePermissionDto dto)
    {
        var permission = await _permissions.GetByIdAsync(id);
        if (permission == null)
            throw new KeyNotFoundException(Messages.Permission.NotFound);

        var normalized = BuildPermission(dto);

        if (await _permissions.AnyAsync(p => p.Code == normalized.Code && p.Id != id))
            throw new InvalidOperationException(Messages.Permission.CodeAlreadyExists);

        if (await _permissions.AnyAsync(p =>
                p.Id != id &&
                p.ControllerName == normalized.ControllerName &&
                p.ActionName == normalized.ActionName))
            throw new InvalidOperationException(Messages.Permission.AlreadyExistsForAction);

        permission.Name = normalized.Name;
        permission.Code = normalized.Code;
        permission.Description = normalized.Description;
        permission.ControllerName = normalized.ControllerName;
        permission.ActionName = normalized.ActionName;
        permission.CRUDActionType = normalized.CRUDActionType;
        permission.IsActive = dto.IsActive;
        permission.UpdatedDate = DateTime.UtcNow;

        await _permissions.UpdateAsync(permission);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _permissions.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<PermissionGroupDto>> GetGroupedPermissionsAsync(int? roleId = null)
    {
        var allPermissions = await _permissions.Query()
            .Where(p => p.IsActive)
            .OrderBy(p => p.ControllerName)
            .ThenBy(p => p.CRUDActionType)
            .ToListAsync();

        var rolePermissionIds = roleId.HasValue
            ? await _rolePermissions.Query()
                .Where(rp => rp.RoleId == roleId.Value)
                .Select(rp => rp.PermissionId)
                .ToListAsync()
            : new List<int>();

        return PermissionGroupHelper.BuildGroups(allPermissions, rolePermissionIds);
    }

    public async Task SaveRolePermissionsAsync(int roleId, List<PermissionGroupDto> permissionGroups)
    {
        var existingRolePermissions = await _rolePermissions.Query()
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();

        _rolePermissions.RemoveRange(existingRolePermissions);

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

                var fallbackIds = await _permissions.Query()
                    .Where(p => p.ControllerName == group.ControllerName && crudTypes.Contains(p.CRUDActionType) && p.IsActive)
                    .Select(p => p.Id)
                    .ToListAsync();

                selectedPermissionIds.AddRange(fallbackIds);
            }
        }

        var distinctSelectedPermissionIds = selectedPermissionIds.Distinct().ToList();

        var validPermissionIds = await _permissions.Query()
            .Where(p => distinctSelectedPermissionIds.Contains(p.Id) && p.IsActive)
            .Select(p => p.Id)
            .ToListAsync();

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
