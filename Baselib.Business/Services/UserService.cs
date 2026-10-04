using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Baselib.Core.Constants;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Baselib.Business.Services;

public class UserService : IUserService
{
    private readonly IEntityRepository<User> _users;
    private readonly IRepository<UserRole> _userRoles;
    private readonly IEntityRepository<Role> _roles;
    private readonly ISessionService _sessions;
    private readonly IRoleSecurityService _roleSecurity;
    private readonly IPermissionCheckService _permissionCheckService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public UserService(
        IEntityRepository<User> users,
        IRepository<UserRole> userRoles,
        IEntityRepository<Role> roles,
        ISessionService sessions,
        IRoleSecurityService roleSecurity,
        IPermissionCheckService permissionCheckService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _users = users;
        _userRoles = userRoles;
        _roles = roles;
        _sessions = sessions;
        _roleSecurity = roleSecurity;
        _permissionCheckService = permissionCheckService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _users.GetAllAsync(
            predicate: item => !item.IsDeleted,
            ignoreQueryFilters: true,
            include: q => q.Include(u => u.Department).Include(u => u.UserRoles).ThenInclude(ur => ur.Role),
            asNoTracking: true, cancellationToken: cancellationToken);

        return DataResult<IEnumerable<UserDto>>.Ok(
            users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).Select(u => MapUserToDto(u, null)));
    }

    public async Task<IDataResult<UserDto>> GetByIdAsync(int id, int? activeRoleId = null, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(
            id,
            ignoreQueryFilters: true,
            include: q => q.Include(u => u.Department).Include(u => u.UserRoles).ThenInclude(ur => ur.Role), cancellationToken: cancellationToken);

        if (user == null || user.IsDeleted)
            return DataResult<UserDto>.NotFound(Messages.User.NotFound);

        return DataResult<UserDto>.Ok(MapUserToDto(user, activeRoleId));
    }

    public async Task<IDataResult<UserDto>> CreateAsync(CreateUserDto dto, System.Security.Claims.ClaimsPrincipal? principal = null, CancellationToken cancellationToken = default)
    {
        if (!TryNormalizeIdentity(dto.Username, dto.Email, out var username, out var email, out var normalizedUsername, out var normalizedEmail))
            return DataResult<UserDto>.BadRequest(Messages.General.Required);

        if (!PasswordHelper.MeetsPolicy(dto.Password))
            return DataResult<UserDto>.BadRequest(Messages.User.PasswordPolicyNotMet);

        var roleValidation = await _roleSecurity.ValidateAssignmentAsync(principal, dto.RoleIds, cancellationToken: cancellationToken);
        if (!roleValidation.Success)
            return DataResult<UserDto>.ErrorDataResult(roleValidation.Message, roleValidation.StatusCode);

        if (await _users.AnyAsync(u => u.NormalizedUsername == normalizedUsername, ignoreQueryFilters: true, cancellationToken: cancellationToken))
            return DataResult<UserDto>.BadRequest(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, ignoreQueryFilters: true, cancellationToken: cancellationToken))
            return DataResult<UserDto>.BadRequest(Messages.User.EmailAlreadyExists);

        var user = new User
        {
            Username = username,
            NormalizedUsername = normalizedUsername,
            Email = email,
            NormalizedEmail = normalizedEmail,
            PasswordHash = PasswordHelper.Hash(dto.Password),
            FirstName = dto.FirstName?.Trim(),
            LastName = dto.LastName?.Trim(),
            Phone = dto.Phone?.Trim(),
            DepartmentId = dto.DepartmentId,
            CreatedDate = _timeProvider.GetUtcNow().UtcDateTime,
            IsActive = true
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken);
        try
        {
            await _users.AddAsync(user, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

            await ReplaceUserRolesAsync(user.Id, dto.RoleIds, cancellationToken: cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionSafelyAsync();
            throw;
        }

        var createdResult = await GetByIdAsync(user.Id, cancellationToken: cancellationToken);
        return DataResult<UserDto>.Created(createdResult.Data!, Messages.General.Saved);
    }

    public async Task<IDataResult<UserDto>> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
    {
        // Anonim istek hiçbir zaman istemcinin seçtiği rol veya departmanı kullanmaz.
        // Hesap, yetkili bir yönetici rol atayana kadar hiçbir RBAC rolü taşımaz.

        return await CreateAsync(new CreateUserDto
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            RoleIds = []
        }, cancellationToken: cancellationToken);
    }

    public async Task<IResult> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken);
        if (user == null || user.IsDeleted)
            return Result.NotFound(Messages.User.NotFound);

        if (!TryNormalizeIdentity(dto.Username, dto.Email, out var username, out var email, out var normalizedUsername, out var normalizedEmail))
            return Result.BadRequest(Messages.General.Required);

        if (await _users.AnyAsync(
                u => u.NormalizedUsername == normalizedUsername && u.Id != id,
                ignoreQueryFilters: true, cancellationToken: cancellationToken))
            return Result.BadRequest(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(
                u => u.NormalizedEmail == normalizedEmail && u.Id != id,
                ignoreQueryFilters: true, cancellationToken: cancellationToken))
            return Result.BadRequest(Messages.User.EmailAlreadyExists);

        if (!dto.IsActive && await WouldRemoveLastPrivilegedAdminAsync(id, null, userWillRemainActive: false, cancellationToken: cancellationToken))
            return Result.BadRequest(Messages.User.LastPrivilegedAdminMustRemain);

        // Complete validation before changing the tracked user.
        user.Username = username;
        user.NormalizedUsername = normalizedUsername;
        user.Email = email;
        user.NormalizedEmail = normalizedEmail;
        user.FirstName = dto.FirstName?.Trim();
        user.LastName = dto.LastName?.Trim();
        user.Phone = dto.Phone?.Trim();
        user.DepartmentId = dto.DepartmentId;
        if (user.IsActive && !dto.IsActive)
            await RevokeUserSessionsAsync(user.Id, "AccountDisabled", cancellationToken: cancellationToken);
        user.IsActive = dto.IsActive;
        user.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> ResetPasswordAsync(
        System.Security.Claims.ClaimsPrincipal principal, int id, ResetUserPasswordDto dto, CancellationToken cancellationToken = default)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return Result.Unauthorized("Oturum bulunamadı.");

        var callerUserId = ClaimsPrincipalHelper.GetUserId(principal);
        var activeRoleId = ClaimsPrincipalHelper.GetActiveRoleId(principal);
        if (!await _permissionCheckService.HasAccessAsync(
                callerUserId, activeRoleId, Constants.Permissions.UsersResetPassword, cancellationToken: cancellationToken))
            return Result.Forbidden(Messages.User.PasswordResetNotAllowed);

        var user = await _users.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken);
        if (user == null || user.IsDeleted)
            return Result.NotFound(Messages.User.NotFound);

        // Inactive accounts/roles must not bypass protection of a privileged account.
        var isPrivileged = await _roleSecurity.HasPrivilegedRoleAsync(id, cancellationToken: cancellationToken);
        if (isPrivileged && !await _permissionCheckService.HasAccessAsync(
                callerUserId, activeRoleId, Constants.Permissions.UsersResetPrivilegedPassword, cancellationToken: cancellationToken))
            return Result.Forbidden(Messages.User.PrivilegedPasswordResetNotAllowed);

        if (!PasswordHelper.MeetsPolicy(dto.NewPassword))
            return Result.BadRequest(Messages.User.PasswordPolicyNotMet);

        var passwordHash = PasswordHelper.Hash(dto.NewPassword);
        user.PasswordHash = passwordHash;
        user.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;
        await RevokeUserSessionsAsync(user.Id, "PasswordResetByAdministrator", cancellationToken: cancellationToken);
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
        return Result.Ok(Messages.User.PasswordReset);
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, ignoreQueryFilters: true, cancellationToken: cancellationToken);
        if (user == null || user.IsDeleted)
            return Result.NotFound(Messages.User.NotFound);

        if (await WouldRemoveLastPrivilegedAdminAsync(id, null, userWillRemainActive: false, cancellationToken: cancellationToken))
            return Result.BadRequest(Messages.User.LastPrivilegedAdminMustRemain);

        await RevokeUserSessionsAsync(user.Id, "AccountDisabled", cancellationToken: cancellationToken);
        user.IsDeleted = true;
        user.IsActive = false;
        user.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

        return Result.Ok(Messages.General.Deleted);
    }

    public async Task<IResult> AssignRolesAsync(
        System.Security.Claims.ClaimsPrincipal principal,
        int userId,
        List<int> roleIds, CancellationToken cancellationToken = default)
    {
        if (!await _users.AnyAsync(u => u.Id == userId && !u.IsDeleted, ignoreQueryFilters: true, cancellationToken: cancellationToken))
            return Result.NotFound(Messages.User.NotFound);

        var roleValidation = await _roleSecurity.ValidateAssignmentAsync(principal, roleIds, userId, cancellationToken: cancellationToken);
        if (!roleValidation.Success)
            return roleValidation;

        if (await WouldRemoveLastPrivilegedAdminAsync(userId, roleIds, userWillRemainActive: true, cancellationToken: cancellationToken))
            return Result.BadRequest(Messages.User.LastPrivilegedAdminMustRemain);

        var currentRoles = await _userRoles.GetAllAsync(ur => ur.UserId == userId,
            ignoreQueryFilters: true, asNoTracking: true, cancellationToken: cancellationToken);
        if (currentRoles.Select(ur => ur.RoleId).ToHashSet().SetEquals(roleIds))
            return Result.Ok(Messages.General.Saved);

        await ReplaceUserRolesAsync(userId, roleIds, cancellationToken: cancellationToken);
        await RevokeUserSessionsAsync(userId, "RolesChanged", cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

        return Result.Ok(Messages.General.Saved);
    }

    public async Task<IResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken: cancellationToken);
        if (user == null || user.IsDeleted)
            return Result.NotFound(Messages.User.NotFound);

        if (!PasswordHelper.Verify(currentPassword, user.PasswordHash))
            return Result.Unauthorized(Messages.User.WrongPassword);

        if (!PasswordHelper.MeetsPolicy(newPassword))
            return Result.BadRequest(Messages.User.PasswordPolicyNotMet);

        user.PasswordHash = PasswordHelper.Hash(newPassword);
        user.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

        _users.Update(user);
        await RevokeUserSessionsAsync(user.Id, "PasswordChanged", cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

        return Result.Ok(Messages.User.PasswordChanged);
    }

    public Task RevokeUserSessionsAsync(int userId, string reason, CancellationToken cancellationToken = default) => _sessions.RevokeAllAsync(userId, reason, cancellationToken: cancellationToken);

    private async Task<bool> WouldRemoveLastPrivilegedAdminAsync(
        int userId,
        IEnumerable<int>? requestedRoleIds,
        bool userWillRemainActive, CancellationToken cancellationToken = default)
    {
        var currentRoleIds = (await _userRoles.GetAllAsync(userRole => userRole.UserId == userId, cancellationToken: cancellationToken))
            .Select(userRole => userRole.RoleId)
            .ToHashSet();
        var privilegedRoleIds = (await _roles.GetAllAsync(role => role.IsPrivileged, asNoTracking: true, cancellationToken: cancellationToken))
            .Select(role => role.Id)
            .ToHashSet();

        if (!currentRoleIds.Overlaps(privilegedRoleIds))
            return false;

        var targetRetainsPrivilegedRole = userWillRemainActive &&
            requestedRoleIds?.Any(privilegedRoleIds.Contains) == true;
        if (targetRetainsPrivilegedRole)
            return false;

        return !await _userRoles.AnyAsync(userRole =>
            userRole.UserId != userId &&
            userRole.User.IsActive &&
            userRole.Role.IsActive &&
            userRole.Role.IsPrivileged, cancellationToken: cancellationToken);
    }

    // ── Private Helpers ──────────────────────────────────────────

    private async Task ReplaceUserRolesAsync(int userId, IEnumerable<int> roleIds, CancellationToken cancellationToken = default)
    {
        var existingRoles = await _userRoles.GetAllAsync(ur => ur.UserId == userId, ignoreQueryFilters: true, cancellationToken: cancellationToken);

        _userRoles.RemoveRange(existingRoles);

        var newRoles = roleIds
            .Distinct()
            .Select(roleId => new UserRole { UserId = userId, RoleId = roleId })
            .ToList();

        if (newRoles.Count > 0)
            await _userRoles.AddRangeAsync(newRoles, cancellationToken: cancellationToken);
    }

    private UserDto MapUserToDto(User user, int? activeRoleId)
    {
        var dto = _mapper.Map<UserDto>(user);

        var activeRole = activeRoleId.HasValue
            ? user.UserRoles.FirstOrDefault(ur => ur.RoleId == activeRoleId.Value && !ur.Role.IsDeleted && ur.Role.IsActive)?.Role
            : user.UserRoles.FirstOrDefault(ur => !ur.Role.IsDeleted && ur.Role.IsActive)?.Role;

        if (activeRole != null)
        {
            dto.ActiveRoleId = activeRole.Id;
            dto.ActiveRoleName = activeRole.Name;
        }

        return dto;
    }

    private static bool TryNormalizeIdentity(
        string? usernameInput,
        string? emailInput,
        out string username,
        out string email,
        out string normalizedUsername,
        out string normalizedEmail)
    {
        username = usernameInput?.Trim() ?? string.Empty;
        email = emailInput?.Trim() ?? string.Empty;
        normalizedUsername = UserIdentityHelper.Normalize(username);
        normalizedEmail = UserIdentityHelper.Normalize(email);

        return username.Length is >= 3 and <= 100 &&
               email.Length <= 254 &&
               new EmailAddressAttribute().IsValid(email);
    }
}
