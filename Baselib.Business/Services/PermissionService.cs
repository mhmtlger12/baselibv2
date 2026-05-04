using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class PermissionService : IPermissionService
{
    private readonly IRepository<Permission> _permissions;
    private readonly IRepository<RolePermission> _rolePermissions;
    private readonly IUnitOfWork _unitOfWork;

    public PermissionService(
        IRepository<Permission> permissions,
        IRepository<RolePermission> rolePermissions,
        IUnitOfWork unitOfWork)
    {
        _permissions = permissions;
        _rolePermissions = rolePermissions;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var permissions = await _permissions.Query()
            .Where(p => p.IsActive)
            .OrderBy(p => p.ControllerName)
            .ThenBy(p => p.CRUDActionType)
            .ToListAsync();

        return permissions.Select(MapToDto);
    }

    public async Task<PermissionDto?> GetByIdAsync(int id)
    {
        var permission = await _permissions.GetByIdAsync(id);
        return permission == null ? null : MapToDto(permission);
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
    {
        var permission = BuildPermission(dto);

        if (await _permissions.AnyAsync(p => p.Code == permission.Code))
            throw new InvalidOperationException("Permission code already exists");

        if (await _permissions.AnyAsync(p =>
                p.ControllerName == permission.ControllerName &&
                p.ActionName == permission.ActionName))
            throw new InvalidOperationException("Permission already exists for this controller/action");

        await _permissions.AddAsync(permission);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(permission);
    }

    public async Task UpdateAsync(int id, CreatePermissionDto dto)
    {
        var permission = await _permissions.GetByIdAsync(id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found");

        var normalized = BuildPermission(dto);

        if (await _permissions.AnyAsync(p => p.Code == normalized.Code && p.Id != id))
            throw new InvalidOperationException("Permission code already exists");

        if (await _permissions.AnyAsync(p =>
                p.Id != id &&
                p.ControllerName == normalized.ControllerName &&
                p.ActionName == normalized.ActionName))
            throw new InvalidOperationException("Permission already exists for this controller/action");

        permission.Name = normalized.Name;
        permission.Code = normalized.Code;
        permission.Description = normalized.Description;
        permission.ControllerName = normalized.ControllerName;
        permission.ActionName = normalized.ActionName;
        permission.CRUDActionType = normalized.CRUDActionType;
        permission.IsActive = dto.IsActive;
        permission.UpdatedDate = DateTime.Now;

        await _permissions.UpdateAsync(permission);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var permission = await _permissions.GetByIdAsync(id);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found");

        permission.IsActive = false;
        permission.UpdatedDate = DateTime.Now;
        await _permissions.UpdateAsync(permission);
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

        return BuildPermissionGroups(allPermissions, rolePermissionIds);
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
            CreatedDate = DateTime.Now,
            IsActive = dto.IsActive
        };
    }

    private static List<PermissionGroupDto> BuildPermissionGroups(IEnumerable<Permission> permissions, IReadOnlyCollection<int> selectedIds)
    {
        return permissions
            .GroupBy(p => p.ControllerName)
            .OrderBy(g => g.Key)
            .Select(group =>
            {
                var items = group
                    .OrderBy(p => p.CRUDActionType)
                    .ThenBy(p => p.ActionName)
                    .Select(permission =>
                    {
                        return new ControllerCrudDto
                        {
                            PermissionId = permission.Id,
                            CRUDActionType = permission.CRUDActionType,
                            Name = GetPermissionActionName(permission),
                            ActionName = permission.ActionName,
                            Code = permission.Code,
                            Checked = selectedIds.Contains(permission.Id)
                        };
                    })
                    .ToList();

                return new PermissionGroupDto
                {
                    ControllerName = group.Key,
                    ControllerCrudList = items,
                    Checked = items.All(c => c.Checked),
                    Indeterminate = items.Any(c => c.Checked) && !items.All(c => c.Checked)
                };
            })
            .ToList();
    }

    private static string GetPermissionActionName(Permission permission)
    {
        var crudName = CRUDActionTypes.GetName(permission.CRUDActionType);
        return crudName == permission.CRUDActionType.ToString()
            ? permission.ActionName
            : crudName;
    }

    private static PermissionDto MapToDto(Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Code = permission.Code,
            Description = permission.Description,
            ControllerName = permission.ControllerName,
            ActionName = permission.ActionName,
            CRUDActionType = permission.CRUDActionType,
            IsActive = permission.IsActive
        };
    }
}
