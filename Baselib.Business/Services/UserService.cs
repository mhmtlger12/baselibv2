using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public UserService(
        IRepository<User> users,
        IRepository<UserRole> userRoles,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _users = users;
        _userRoles = userRoles;
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

    public async Task<IDataResult<UserDto>> CreateAsync(CreateUserDto dto)
    {
        if (!TryNormalizeIdentity(dto.Username, dto.Email, out var username, out var email, out var normalizedUsername, out var normalizedEmail))
            return DataResult<UserDto>.BadRequest(Messages.General.Required);

        if (!PasswordHelper.MeetsPolicy(dto.Password))
            return DataResult<UserDto>.BadRequest(Messages.User.PasswordPolicyNotMet);

        if (await _users.AnyAsync(u => u.NormalizedUsername == normalizedUsername))
            return DataResult<UserDto>.BadRequest(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
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

        if (await _users.AnyAsync(u => u.NormalizedUsername == normalizedUsername && u.Id != id))
            return Result.BadRequest(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(u => u.NormalizedEmail == normalizedEmail && u.Id != id))
            return Result.BadRequest(Messages.User.EmailAlreadyExists);

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
        }

        _users.Update(user);
        await ReplaceUserRolesAsync(id, dto.RoleIds);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Updated);
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return Result.NotFound(Messages.User.NotFound);

        user.IsActive = false;
        user.UpdatedDate = _timeProvider.GetUtcNow().UtcDateTime;
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.General.Deleted);
    }

    public async Task<IResult> AssignRolesAsync(int userId, List<int> roleIds)
    {
        if (!await _users.AnyAsync(u => u.Id == userId))
            return Result.NotFound(Messages.User.NotFound);

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
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.User.PasswordChanged);
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
