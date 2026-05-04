using Microsoft.EntityFrameworkCore;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Data;
using Baselib.Entities;

namespace Baselib.Business.Services;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;

    public PermissionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var permissions = await _context.Permissions
            .Where(p => p.IsActive)
            .ToListAsync();

        return permissions.Select(MapToDto);
    }

    public async Task<PermissionDto?> GetByIdAsync(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        return permission == null ? null : MapToDto(permission);
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
    {
        if (await _context.Permissions.AnyAsync(p => p.Code == dto.Code))
            throw new KeyNotFoundException("Permission code already exists");

        var permission = new Permission
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            ControllerName = dto.ControllerName ?? "",
            ActionName = dto.ActionName ?? "",
            CRUDActionType = dto.CRUDActionType,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        return MapToDto(permission);
    }

    public async Task UpdateAsync(int id, CreatePermissionDto dto)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null) throw new KeyNotFoundException("Permission not found");

        if (await _context.Permissions.AnyAsync(p => p.Code == dto.Code && p.Id != id))
            throw new InvalidOperationException("Permission code already exists");

        permission.Name = dto.Name;
        permission.Code = dto.Code;
        permission.Description = dto.Description;
        permission.ControllerName = dto.ControllerName ?? permission.ControllerName;
        permission.ActionName = dto.ActionName ?? permission.ActionName;
        permission.CRUDActionType = dto.CRUDActionType;
        permission.UpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);
        if (permission == null) throw new KeyNotFoundException("Permission not found");

        permission.IsActive = false;
        permission.UpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PermissionGroupDto>> GetGroupedPermissionsAsync(int? roleId = null)
    {
        var allPermissions = await _context.Permissions
            .Where(p => p.IsActive)
            .ToListAsync();

        var rolePermissionIds = new List<int>();
        if (roleId.HasValue)
        {
            rolePermissionIds = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId.Value)
                .Select(rp => rp.PermissionId)
                .ToListAsync();
        }

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

    public async Task SaveRolePermissionsAsync(int roleId, List<PermissionGroupDto> permissionGroups)
    {
        var existingRolePermissions = _context.RolePermissions.Where(rp => rp.RoleId == roleId);
        _context.RolePermissions.RemoveRange(existingRolePermissions);

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
                        RoleId = roleId,
                        PermissionId = permission.Id
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
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