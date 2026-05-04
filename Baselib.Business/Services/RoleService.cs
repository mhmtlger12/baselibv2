using Microsoft.EntityFrameworkCore;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Data;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.IsActive)
            .ToListAsync();

        return roles.Select(MapToDto);
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

        return role == null ? null : MapToDto(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        if (await _context.Roles.AnyAsync(r => r.Name == dto.Name))
            throw new InvalidOperationException("Role name already exists");

        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        if (dto.PermissionIds.Any())
        {
            foreach (var permissionId in dto.PermissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }
            await _context.SaveChangesAsync();
        }

        return (await GetByIdAsync(role.Id))!;
    }

    public async Task UpdateAsync(int id, UpdateRoleDto dto)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) throw new KeyNotFoundException("Role not found");

        if (await _context.Roles.AnyAsync(r => r.Name == dto.Name && r.Id != id))
            throw new InvalidOperationException("Role name already exists");

        role.Name = dto.Name;
        role.Description = dto.Description;
        role.IsActive = dto.IsActive;
        role.UpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) throw new KeyNotFoundException("Role not found");

        role.IsActive = false;
        role.UpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task AssignPermissionsAsync(int roleId, List<int> permissionIds)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null) throw new KeyNotFoundException("Role not found");

        var existingPermissions = _context.RolePermissions.Where(rp => rp.RoleId == roleId);
        _context.RolePermissions.RemoveRange(existingPermissions);

        foreach (var permissionId in permissionIds)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PermissionGroupDto>> GetPermissionsByRoleIdAsync(int roleId)
    {
        var allPermissions = await _context.Permissions
            .Where(p => p.IsActive)
            .ToListAsync();

        var rolePermissionIds = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var controllerNames = allPermissions
            .Select(p => p.ControllerName)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        var result = new List<PermissionGroupDto>();

        foreach (var controller in controllerNames)
        {
            var controllerPermissions = allPermissions
                .Where(p => p.ControllerName == controller)
                .ToList();

            var group = new PermissionGroupDto
            {
                ControllerName = controller,
                ControllerCrudList = new List<ControllerCrudDto>()
            };

            var crudTypes = new[] { CRUDActionTypes.Create, CRUDActionTypes.Read, CRUDActionTypes.Update, CRUDActionTypes.Delete };

            foreach (var crud in crudTypes)
            {
                var permission = controllerPermissions.FirstOrDefault(p => p.CRUDActionType == crud);
                var isChecked = permission != null && rolePermissionIds.Contains(permission.Id);

                group.ControllerCrudList.Add(new ControllerCrudDto
                {
                    CRUDActionType = crud,
                    Name = CRUDActionTypes.GetName(crud),
                    Checked = isChecked
                });
            }

            group.Checked = group.ControllerCrudList.All(c => c.Checked);
            group.Indeterminate = group.ControllerCrudList.Any(c => c.Checked) && !group.Checked;

            result.Add(group);
        }

        return result;
    }

    public async Task UpdateWithPermissionsAsync(int id, UpdateRoleDto dto, List<PermissionGroupDto> permissionGroups)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) throw new KeyNotFoundException("Role not found");

        if (await _context.Roles.AnyAsync(r => r.Name == dto.Name && r.Id != id))
            throw new InvalidOperationException("Role name already exists");

        role.Name = dto.Name;
        role.Description = dto.Description;
        role.IsActive = dto.IsActive;
        role.UpdatedDate = DateTime.Now;

        var existingPermissions = _context.RolePermissions.Where(rp => rp.RoleId == id);
        _context.RolePermissions.RemoveRange(existingPermissions);

        foreach (var group in permissionGroups.Where(p => p.Checked || p.ControllerCrudList.Any(c => c.Checked)))
        {
            foreach (var crud in group.ControllerCrudList.Where(c => c.Checked))
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p =>
                        p.ControllerName == group.ControllerName &&
                        p.CRUDActionType == crud.CRUDActionType &&
                        p.IsActive);

                if (permission != null)
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = id,
                        PermissionId = permission.Id
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
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