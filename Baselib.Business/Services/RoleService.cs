using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roles;
    private readonly IRepository<Permission> _permissions;
    private readonly IRepository<RolePermission> _rolePermissions;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(
        IRepository<Role> roles,
        IRepository<Permission> permissions,
        IRepository<RolePermission> rolePermissions,
        IUnitOfWork unitOfWork)
    {
        _roles = roles;
        _permissions = permissions;
        _rolePermissions = rolePermissions;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _roles.Query()
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return roles.Select(MapToDto);
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _roles.Query()
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

        return role == null ? null : MapToDto(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        if (await _roles.AnyAsync(r => r.Name == dto.Name.Trim()))
            throw new InvalidOperationException("Role name already exists");

        var role = new Role
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        await _roles.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        await ReplaceRolePermissionsAsync(role.Id, dto.PermissionIds);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(role.Id))!;
    }

    public async Task UpdateAsync(int id, UpdateRoleDto dto)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            throw new KeyNotFoundException("Role not found");

        if (await _roles.AnyAsync(r => r.Name == dto.Name.Trim() && r.Id != id))
            throw new InvalidOperationException("Role name already exists");

        role.Name = dto.Name.Trim();
        role.Description = dto.Description?.Trim();
        role.IsActive = dto.IsActive;
        role.UpdatedDate = DateTime.Now;

        await _roles.UpdateAsync(role);
        await ReplaceRolePermissionsAsync(id, dto.PermissionIds);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            throw new KeyNotFoundException("Role not found");

        role.IsActive = false;
        role.UpdatedDate = DateTime.Now;
        await _roles.UpdateAsync(role);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignPermissionsAsync(int roleId, List<int> permissionIds)
    {
        if (!await _roles.AnyAsync(r => r.Id == roleId))
            throw new KeyNotFoundException("Role not found");

        await ReplaceRolePermissionsAsync(roleId, permissionIds);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<PermissionGroupDto>> GetPermissionsByRoleIdAsync(int roleId)
    {
        var allPermissions = await _permissions.Query()
            .Where(p => p.IsActive)
            .OrderBy(p => p.ControllerName)
            .ThenBy(p => p.CRUDActionType)
            .ToListAsync();

        var rolePermissionIds = await _rolePermissions.Query()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        return BuildPermissionGroups(allPermissions, rolePermissionIds);
    }

    public async Task UpdateWithPermissionsAsync(int id, UpdateRoleDto dto, List<PermissionGroupDto> permissionGroups)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            throw new KeyNotFoundException("Role not found");

        if (await _roles.AnyAsync(r => r.Name == dto.Name.Trim() && r.Id != id))
            throw new InvalidOperationException("Role name already exists");

        role.Name = dto.Name.Trim();
        role.Description = dto.Description?.Trim();
        role.IsActive = dto.IsActive;
        role.UpdatedDate = DateTime.Now;

        await _roles.UpdateAsync(role);

        var permissionIds = await ResolvePermissionIdsAsync(permissionGroups);
        await ReplaceRolePermissionsAsync(id, permissionIds);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ReplaceRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds)
    {
        var existingPermissions = await _rolePermissions.Query()
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();

        _rolePermissions.RemoveRange(existingPermissions);

        var newPermissions = permissionIds
            .Distinct()
            .Select(permissionId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            })
            .ToList();

        if (newPermissions.Count > 0)
            await _rolePermissions.AddRangeAsync(newPermissions);
    }

    private async Task<List<int>> ResolvePermissionIdsAsync(List<PermissionGroupDto> permissionGroups)
    {
        var selected = permissionGroups
            .SelectMany(group => group.ControllerCrudList)
            .Where(crud => crud.Checked && crud.PermissionId > 0)
            .Select(crud => crud.PermissionId)
            .Distinct()
            .ToList();

        if (selected.Any())
            return selected;

        foreach (var group in permissionGroups.Where(p => p.Checked || p.ControllerCrudList.Any(c => c.Checked)))
        {
            var crudTypes = group.ControllerCrudList
                .Where(c => c.Checked)
                .Select(c => c.CRUDActionType)
                .ToList();

            var permissionIds = await _permissions.Query()
                .Where(p => p.ControllerName == group.ControllerName && crudTypes.Contains(p.CRUDActionType) && p.IsActive)
                .Select(p => p.Id)
                .ToListAsync();

            selected.AddRange(permissionIds);
        }

        return selected.Distinct().ToList();
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

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = role.RolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                Code = rp.Permission.Code,
                Description = rp.Permission.Description,
                ControllerName = rp.Permission.ControllerName,
                ActionName = rp.Permission.ActionName,
                CRUDActionType = rp.Permission.CRUDActionType,
                IsActive = rp.Permission.IsActive
            }).ToList(),
            IsActive = role.IsActive,
            CreatedDate = role.CreatedDate
        };
    }
}
