using System.Security.Claims;
using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Constants;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class RoleService : IRoleService
{
    private readonly IEntityRepository<Role> _roles;
    private readonly IRepository<RolePermission> _rolePermissions;
    private readonly IEntityRepository<Permission> _permissions;
    private readonly IPermissionService _permissionService;
    private readonly IRoleSecurityService _roleSecurity;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public RoleService(
        IEntityRepository<Role> roles,
        IRepository<RolePermission> rolePermissions,
        IEntityRepository<Permission> permissions,
        IPermissionService permissionService,
        IRoleSecurityService roleSecurity,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _roles = roles;
        _rolePermissions = rolePermissions;
        _permissions = permissions;
        _permissionService = permissionService;
        _roleSecurity = roleSecurity;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<RoleDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roles.GetAllAsync(
            predicate: item => !item.IsDeleted,
            ignoreQueryFilters: true,
            include: q => q.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission),
            asNoTracking: true, cancellationToken: cancellationToken);

        return DataResult<IEnumerable<RoleDto>>.Ok(_mapper.Map<IEnumerable<RoleDto>>(roles.OrderBy(r => r.Name)));
    }

    public async Task<IDataResult<IEnumerable<SelectOptionDto>>> GetSelectOptionsAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roles.GetAllAsync(asNoTracking: true, cancellationToken: cancellationToken);
        var options = roles.OrderBy(r => r.Name).Select(r => new SelectOptionDto
        {
            Id = r.Id,
            Name = r.Name
        });

        return DataResult<IEnumerable<SelectOptionDto>>.Ok(options);
    }

    public async Task<IDataResult<RoleDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(
            id,
            ignoreQueryFilters: true,
            include: q => q.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission), cancellationToken: cancellationToken);

        if (role == null || role.IsDeleted)
            return DataResult<RoleDto>.NotFound(Messages.Role.NotFound);

        return DataResult<RoleDto>.Ok(_mapper.Map<RoleDto>(role));
    }

    public async Task<IDataResult<RoleDto>> CreateAsync(ClaimsPrincipal principal, CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var roleName = dto.Name.Trim();
        if (await _roles.AnyAsync(r => r.Name == roleName, ignoreQueryFilters: true, cancellationToken: cancellationToken))
            return DataResult<RoleDto>.BadRequest(Messages.Role.NameAlreadyExists);

        var validation = await _roleSecurity.ValidateManagementAsync(principal, null, dto.PermissionIds, cancellationToken: cancellationToken);
        if (!validation.Success)
            return DataResult<RoleDto>.ErrorDataResult(validation.Message, validation.StatusCode);

        var role = new Role
        {
            Name = roleName,
            IsPrivileged = validation.Data,
            Description = dto.Description?.Trim(),
            CreatedDate = _timeProvider.GetUtcNow().UtcDateTime,
            IsActive = true
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);
        try
        {
            await _roles.AddAsync(role, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

            await ReplaceRolePermissionsAsync(role.Id, dto.PermissionIds, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionSafelyAsync();
            throw;
        }

        var created = await GetByIdAsync(role.Id, cancellationToken: cancellationToken);
        return DataResult<RoleDto>.Created(created.Data!, Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(ClaimsPrincipal principal, int id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken);
        if (role == null || role.IsDeleted)
            return Result.NotFound(Messages.Role.NotFound);

        var roleName = dto.Name.Trim();
        if (await _roles.AnyAsync(r => r.Name == roleName && r.Id != id, ignoreQueryFilters: true, cancellationToken: cancellationToken))
            return Result.BadRequest(Messages.Role.NameAlreadyExists);

        if (role.IsSystemRole && !dto.IsActive)
            return Result.BadRequest(Messages.Role.SystemRoleCannotBeDeleted);

        var validation = await _roleSecurity.ValidateManagementAsync(principal, role, dto.PermissionIds, cancellationToken: cancellationToken);
        if (!validation.Success)
            return Result.ErrorResult(validation.Message, validation.StatusCode);

        if (!await HasRequiredSystemRolePermissionsAsync(role, dto.PermissionIds, cancellationToken: cancellationToken))
            return Result.BadRequest(Messages.Role.SystemRoleCriticalPermissionsRequired);

        role.IsPrivileged = validation.Data;
        role.Name = roleName;
        role.Description = dto.Description?.Trim();
        role.IsActive = dto.IsActive;
        role.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

        _roles.Update(role);
        await ReplaceRolePermissionsAsync(id, dto.PermissionIds, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(ClaimsPrincipal principal, int id, CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken);
        if (role == null || role.IsDeleted)
            return Result.NotFound(Messages.Role.NotFound);

        if (role.IsSystemRole)
            return Result.BadRequest(Messages.Role.SystemRoleCannotBeDeleted);

        var validation = await _roleSecurity.ValidateManagementAsync(principal, role, [], cancellationToken: cancellationToken);
        if (!validation.Success)
            return Result.ErrorResult(validation.Message, validation.StatusCode);

        role.IsDeleted = true;
        role.IsActive = false;
        role.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;
        _roles.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

        return Result.Ok(Messages.General.Deleted);
    }

    public async Task<IResult> AssignPermissionsAsync(ClaimsPrincipal principal, int roleId, List<int> permissionIds, CancellationToken cancellationToken = default)
    {
        var role = await _roles.GetByIdAsync(roleId, ignoreQueryFilters: true, cancellationToken: cancellationToken);
        if (role == null || role.IsDeleted)
            return Result.NotFound(Messages.Role.NotFound);

        var validation = await _roleSecurity.ValidateManagementAsync(principal, role, permissionIds, cancellationToken: cancellationToken);
        if (!validation.Success)
            return Result.ErrorResult(validation.Message, validation.StatusCode);

        if (!await HasRequiredSystemRolePermissionsAsync(role, permissionIds, cancellationToken: cancellationToken))
            return Result.BadRequest(Messages.Role.SystemRoleCriticalPermissionsRequired);

        role.IsPrivileged = validation.Data;
        await ReplaceRolePermissionsAsync(roleId, permissionIds, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

        return Result.Ok(Messages.General.Saved);
    }

    public async Task<IDataResult<IEnumerable<PermissionGroupDto>>> GetPermissionsByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await _permissionService.GetGroupedPermissionsAsync(roleId, cancellationToken: cancellationToken);
    }



    // ── Private Helpers ──────────────────────────────────────────

    private async Task<bool> HasRequiredSystemRolePermissionsAsync(Role role, IEnumerable<int> permissionIds, CancellationToken cancellationToken = default)
    {
        if (!role.IsSystemRole)
            return true;

        var criticalCodes = SecurityPermissions.CriticalCodes.ToArray();
        var permissions = (await _permissions.GetAllAsync(
            permission => criticalCodes.Contains(permission.Code), asNoTracking: true, cancellationToken: cancellationToken)).ToArray();
        var requestedIds = permissionIds.ToHashSet();
        return permissions.Length == criticalCodes.Length && permissions.All(permission => requestedIds.Contains(permission.Id));
    }

    private async Task ReplaceRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds, CancellationToken cancellationToken = default)
    {
        var existingPermissions = await _rolePermissions.GetAllAsync(rp => rp.RoleId == roleId, ignoreQueryFilters: true, cancellationToken: cancellationToken);

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
            await _rolePermissions.AddRangeAsync(newPermissions, cancellationToken: cancellationToken);
    }
}
