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
    private readonly IRepository<User> _users;
    private readonly IRepository<UserRole> _userRoles;
    private readonly IRepository<Role> _roles;
    private readonly IRepository<RefreshToken> _refreshTokens;
    private readonly IPermissionCheckService _permissionCheckService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public UserService(
        IRepository<User> users,
        IRepository<UserRole> userRoles,
        IRepository<Role> roles,
        IRepository<RefreshToken> refreshTokens,
        IPermissionCheckService permissionCheckService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _users = users;
        _userRoles = userRoles;
        _roles = roles;
        _refreshTokens = refreshTokens;
        _permissionCheckService = permissionCheckService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<IEnumerable<UserDto>>> GetAllAsync()
    {
        var users = await _users.GetAllAsync(
            predicate: null,
            include: q => q.Include(u => u.Department).Include(u => u.UserRoles).ThenInclude(ur => ur.Role),
            asNoTracking: true);

        return DataResult<IEnumerable<UserDto>>.Ok(
            users.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).Select(u => MapUserToDto(u, null)));
    }

    public async Task<IDataResult<UserDto>> GetByIdAsync(int id, int? activeRoleId = null)
    {
        var user = await _users.GetByIdAsync(
            id,
            include: q => q.Include(u => u.Department).Include(u => u.UserRoles).ThenInclude(ur => ur.Role));

        if (user == null)
            return DataResult<UserDto>.NotFound(Messages.User.NotFound);

        return DataResult<UserDto>.Ok(MapUserToDto(user, activeRoleId));
    }

    public async Task<IDataResult<UserDto>> CreateAsync(CreateUserDto dto, System.Security.Claims.ClaimsPrincipal? principal = null)
    {
        if (!TryNormalizeIdentity(dto.Username, dto.Email, out var username, out var email, out var normalizedUsername, out var normalizedEmail))
            return DataResult<UserDto>.BadRequest(Messages.General.Required);

        if (!PasswordHelper.MeetsPolicy(dto.Password))
            return DataResult<UserDto>.BadRequest(Messages.User.PasswordPolicyNotMet);

        var roleValidation = await ValidateRoleAssignmentAsync(principal, dto.RoleIds);
        if (!roleValidation.Success)
            return DataResult<UserDto>.ErrorDataResult(roleValidation.Message, roleValidation.StatusCode);

        if (await _users.AnyAsync(u => u.NormalizedUsername == normalizedUsername, ignoreQueryFilters: true))
            return DataResult<UserDto>.BadRequest(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, ignoreQueryFilters: true))
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

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await ReplaceUserRolesAsync(user.Id, dto.RoleIds);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionSafelyAsync();
            throw;
        }

        var createdResult = await GetByIdAsync(user.Id);
        return DataResult<UserDto>.Created(createdResult.Data!, Messages.General.Saved);
    }

    public async Task<IDataResult<UserDto>> RegisterAsync(RegisterUserDto dto)
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
        });
    }

    public async Task<IResult> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return Result.NotFound(Messages.User.NotFound);

        if (!TryNormalizeIdentity(dto.Username, dto.Email, out var username, out var email, out var normalizedUsername, out var normalizedEmail))
            return Result.BadRequest(Messages.General.Required);

        if (await _users.AnyAsync(
                u => u.NormalizedUsername == normalizedUsername && u.Id != id,
                ignoreQueryFilters: true))
            return Result.BadRequest(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(
                u => u.NormalizedEmail == normalizedEmail && u.Id != id,
                ignoreQueryFilters: true))
            return Result.BadRequest(Messages.User.EmailAlreadyExists);

        if (!dto.IsActive && await WouldRemoveLastPrivilegedAdminAsync(id, null, userWillRemainActive: false))
            return Result.BadRequest(Messages.User.LastPrivilegedAdminMustRemain);

        user.Username = username;
        user.NormalizedUsername = normalizedUsername;
        user.Email = email;
        user.NormalizedEmail = normalizedEmail;
        user.FirstName = dto.FirstName?.Trim();
        user.LastName = dto.LastName?.Trim();
        user.Phone = dto.Phone?.Trim();
        user.DepartmentId = dto.DepartmentId;
        user.IsActive = dto.IsActive;
        user.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            if (!PasswordHelper.MeetsPolicy(dto.Password))
                return Result.BadRequest(Messages.User.PasswordPolicyNotMet);

            user.PasswordHash = PasswordHelper.Hash(dto.Password);
            await RevokeUserSessionsAsync(user.Id, "PasswordResetByAdministrator");
        }

        _users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return Result.NotFound(Messages.User.NotFound);

        if (await WouldRemoveLastPrivilegedAdminAsync(id, null, userWillRemainActive: false))
            return Result.BadRequest(Messages.User.LastPrivilegedAdminMustRemain);

        user.IsActive = false;
        user.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Deleted);
    }

    public async Task<IResult> AssignRolesAsync(
        System.Security.Claims.ClaimsPrincipal principal,
        int userId,
        List<int> roleIds)
    {
        if (!await _users.AnyAsync(u => u.Id == userId))
            return Result.NotFound(Messages.User.NotFound);

        var roleValidation = await ValidateRoleAssignmentAsync(principal, roleIds);
        if (!roleValidation.Success)
            return roleValidation;

        if (await WouldRemoveLastPrivilegedAdminAsync(userId, roleIds, userWillRemainActive: true))
            return Result.BadRequest(Messages.User.LastPrivilegedAdminMustRemain);

        await ReplaceUserRolesAsync(userId, roleIds);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Saved);
    }

    public async Task<IResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null)
            return Result.NotFound(Messages.User.NotFound);

        if (!PasswordHelper.Verify(currentPassword, user.PasswordHash))
            return Result.Unauthorized(Messages.User.WrongPassword);

        if (!PasswordHelper.MeetsPolicy(newPassword))
            return Result.BadRequest(Messages.User.PasswordPolicyNotMet);

        user.PasswordHash = PasswordHelper.Hash(newPassword);
        user.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;

        _users.Update(user);
        await RevokeUserSessionsAsync(user.Id, "PasswordChanged");
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.User.PasswordChanged);
    }

    public async Task RevokeUserSessionsAsync(int userId, string reason)
    {
        var activeTokens = await _refreshTokens.GetAllAsync(
            token => token.UserId == userId && !token.RevokedDate.HasValue);

        var revokedAt = _timeProvider.GetUtcNow().UtcDateTime;
        foreach (var token in activeTokens)
        {
            token.RevokedDate = revokedAt;
            token.RevokedReason = reason;
        }
    }

    private async Task<IResult> ValidateRoleAssignmentAsync(
        System.Security.Claims.ClaimsPrincipal? principal,
        IEnumerable<int> roleIds)
    {
        var requestedRoleIds = roleIds.Distinct().ToArray();
        if (requestedRoleIds.Length == 0)
            return Result.Ok();

        var roles = await _roles.GetAllAsync(
            role => requestedRoleIds.Contains(role.Id),
            asNoTracking: true);
        var selectedRoles = roles.ToList();

        if (selectedRoles.Count != requestedRoleIds.Length)
            return Result.BadRequest(Messages.User.InvalidRoleSelection);

        if (principal == null)
            return Result.Forbidden(Messages.User.RoleAssignmentNotAllowed);

        var callerUserId = ClaimsPrincipalHelper.GetUserId(principal);
        var activeRoleId = ClaimsPrincipalHelper.GetActiveRoleId(principal);
        if (!await _permissionCheckService.HasAccessAsync(
                callerUserId,
                activeRoleId,
                Constants.Permissions.UsersAssignRoles))
            return Result.Forbidden(Messages.User.RoleAssignmentNotAllowed);

        if (selectedRoles.Any(role => role.IsPrivileged) &&
            !await _permissionCheckService.HasAccessAsync(
                callerUserId,
                activeRoleId,
                Constants.Permissions.UsersAssignPrivilegedRoles))
        {
            return Result.Forbidden(Messages.User.PrivilegedRoleAssignmentNotAllowed);
        }

        return Result.Ok();
    }

    private async Task<bool> WouldRemoveLastPrivilegedAdminAsync(
        int userId,
        IEnumerable<int>? requestedRoleIds,
        bool userWillRemainActive)
    {
        var currentRoleIds = (await _userRoles.GetAllAsync(userRole => userRole.UserId == userId))
            .Select(userRole => userRole.RoleId)
            .ToHashSet();
        var privilegedRoleIds = (await _roles.GetAllAsync(role => role.IsPrivileged, asNoTracking: true))
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
            userRole.Role.IsPrivileged);
    }

    // ── Private Helpers ──────────────────────────────────────────

    private async Task ReplaceUserRolesAsync(int userId, IEnumerable<int> roleIds)
    {
        var existingRoles = await _userRoles.GetAllAsync(ur => ur.UserId == userId);

        _userRoles.RemoveRange(existingRoles);

        var newRoles = roleIds
            .Distinct()
            .Select(roleId => new UserRole { UserId = userId, RoleId = roleId })
            .ToList();

        if (newRoles.Count > 0)
            await _userRoles.AddRangeAsync(newRoles);
    }

    private UserDto MapUserToDto(User user, int? activeRoleId)
    {
        var dto = _mapper.Map<UserDto>(user);

        var activeRole = activeRoleId.HasValue
            ? user.UserRoles.FirstOrDefault(ur => ur.RoleId == activeRoleId.Value)?.Role
            : user.UserRoles.FirstOrDefault()?.Role;

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
