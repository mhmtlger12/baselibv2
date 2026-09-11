using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Business.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<UserRole> _userRoles;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(
        IRepository<User> users,
        IRepository<UserRole> userRoles,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _users = users;
        _userRoles = userRoles;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<UserDto>>> GetAllAsync()
    {
        var users = await _users.GetAllAsync(
            predicate: null,
            include: q => q.Include(u => u.Department).Include(u => u.UserRoles).ThenInclude(ur => ur.Role));

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
        var username = dto.Username.Trim();
        var email = dto.Email.Trim();

        if (await _users.AnyAsync(u => u.Username == username))
            return DataResult<UserDto>.BadRequest(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(u => u.Email == email))
            return DataResult<UserDto>.BadRequest(Messages.User.EmailAlreadyExists);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = PasswordHelper.Hash(dto.Password),
            FirstName = dto.FirstName?.Trim(),
            LastName = dto.LastName?.Trim(),
            Phone = dto.Phone?.Trim(),
            DepartmentId = dto.DepartmentId,
            CreatedDate = DateTime.UtcNow,
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
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        var createdResult = await GetByIdAsync(user.Id);
        return DataResult<UserDto>.Created(createdResult.Data!, Messages.General.Saved);
    }

    public async Task<IResult> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            return Result.NotFound(Messages.User.NotFound);

        var username = dto.Username.Trim();
        var email = dto.Email.Trim();

        if (await _users.AnyAsync(u => u.Username == username && u.Id != id))
            return Result.BadRequest(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(u => u.Email == email && u.Id != id))
            return Result.BadRequest(Messages.User.EmailAlreadyExists);

        user.Username = username;
        user.Email = email;
        user.FirstName = dto.FirstName?.Trim();
        user.LastName = dto.LastName?.Trim();
        user.Phone = dto.Phone?.Trim();
        user.DepartmentId = dto.DepartmentId;
        user.IsActive = dto.IsActive;
        user.UpdatedDate = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
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
        user.UpdatedDate = DateTime.UtcNow;
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

        user.PasswordHash = PasswordHelper.Hash(newPassword);
        user.UpdatedDate = DateTime.UtcNow;

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
}
