using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roles;
    private readonly IRepository<RolePermission> _rolePermissions;
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoleService(
        IRepository<Role> roles,
        IRepository<RolePermission> rolePermissions,
        IPermissionService permissionService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _roles = roles;
        _rolePermissions = rolePermissions;
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<RoleDto>>> GetAllAsync()
    {
        var roles = await _roles.GetAllAsync(
            predicate: r => r.IsActive,
            include: q => q.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission));

        return DataResult<IEnumerable<RoleDto>>.Ok(_mapper.Map<IEnumerable<RoleDto>>(roles.OrderBy(r => r.Name)));
    }

    public async Task<IDataResult<RoleDto>> GetByIdAsync(int id)
    {
        var role = await _roles.GetByIdAsync(
            id,
            include: q => q.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission));

        if (role == null)
            return DataResult<RoleDto>.NotFound(Messages.Role.NotFound);

        return DataResult<RoleDto>.Ok(_mapper.Map<RoleDto>(role));
    }

    public async Task<IDataResult<RoleDto>> CreateAsync(CreateRoleDto dto)
    {
        var roleName = dto.Name.Trim();
        if (await _roles.AnyAsync(r => r.Name == roleName))
            return DataResult<RoleDto>.BadRequest(Messages.Role.NameAlreadyExists);

        var role = new Role
        {
            Name = roleName,
            Description = dto.Description?.Trim(),
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _roles.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        await ReplaceRolePermissionsAsync(role.Id, dto.PermissionIds);
        await _unitOfWork.SaveChangesAsync();

        var created = await GetByIdAsync(role.Id);
        return DataResult<RoleDto>.Created(created.Data!, Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, UpdateRoleDto dto)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            return Result.NotFound(Messages.Role.NotFound);

        var roleName = dto.Name.Trim();
        if (await _roles.AnyAsync(r => r.Name == roleName && r.Id != id))
            return Result.BadRequest(Messages.Role.NameAlreadyExists);

        role.Name = roleName;
        role.Description = dto.Description?.Trim();
        role.IsActive = dto.IsActive;
        role.UpdatedDate = DateTime.UtcNow;

        _roles.Update(role);
        await ReplaceRolePermissionsAsync(id, dto.PermissionIds);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            return Result.NotFound(Messages.Role.NotFound);

        role.IsActive = false;
        role.UpdatedDate = DateTime.UtcNow;
        _roles.Update(role);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Deleted);
    }

    public async Task<IResult> AssignPermissionsAsync(int roleId, List<int> permissionIds)
    {
        if (!await _roles.AnyAsync(r => r.Id == roleId))
            return Result.NotFound(Messages.Role.NotFound);

        await ReplaceRolePermissionsAsync(roleId, permissionIds);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Saved);
    }

    public async Task<IDataResult<IEnumerable<PermissionGroupDto>>> GetPermissionsByRoleIdAsync(int roleId)
    {
        return await _permissionService.GetGroupedPermissionsAsync(roleId);
    }

    public async Task<IResult> UpdateWithPermissionsAsync(int id, UpdateRoleDto dto, List<PermissionGroupDto> permissionGroups)
    {
        var role = await _roles.GetByIdAsync(id);
        if (role == null)
            return Result.NotFound(Messages.Role.NotFound);

        var roleName = dto.Name.Trim();
        if (await _roles.AnyAsync(r => r.Name == roleName && r.Id != id))
            return Result.BadRequest(Messages.Role.NameAlreadyExists);

        role.Name = roleName;
        role.Description = dto.Description?.Trim();
        role.IsActive = dto.IsActive;
        role.UpdatedDate = DateTime.UtcNow;

        _roles.Update(role);

        var permissionIds = await _permissionService.ResolvePermissionIdsAsync(permissionGroups);
        await ReplaceRolePermissionsAsync(id, permissionIds);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }

    // ── Private Helpers ──────────────────────────────────────────

    private async Task ReplaceRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds)
    {
        var existingPermissions = await _rolePermissions.GetAllAsync(rp => rp.RoleId == roleId);

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
}
