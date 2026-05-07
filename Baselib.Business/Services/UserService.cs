using AutoMapper;
using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Data.Interfaces;
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

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _users.Query()
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();

        return users.Select(u => MapUserToDto(u, null));
    }

    public async Task<UserDto?> GetByIdAsync(int id, int? activeRoleId = null)
    {
        var user = await _users.Query()
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : MapUserToDto(user, activeRoleId);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        await EnsureUniqueUserAsync(dto.Username, dto.Email);

        var user = new User
        {
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim(),
            PasswordHash = PasswordHelper.Hash(dto.Password),
            FirstName = dto.FirstName?.Trim(),
            LastName = dto.LastName?.Trim(),
            Phone = dto.Phone?.Trim(),
            DepartmentId = dto.DepartmentId,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        await _users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await ReplaceUserRolesAsync(user.Id, dto.RoleIds);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(user.Id))!;
    }

    public async Task UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException(Messages.User.NotFound);

        await EnsureUniqueUserAsync(dto.Username, dto.Email, id);

        user.Username = dto.Username.Trim();
        user.Email = dto.Email.Trim();
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

        await _users.UpdateAsync(user);
        await ReplaceUserRolesAsync(id, dto.RoleIds);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _users.SoftDeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignRolesAsync(int userId, List<int> roleIds)
    {
        if (!await _users.AnyAsync(u => u.Id == userId))
            throw new KeyNotFoundException(Messages.User.NotFound);

        await ReplaceUserRolesAsync(userId, roleIds);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException(Messages.User.NotFound);

        if (!PasswordHelper.Verify(currentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException(Messages.User.WrongPassword);

        user.PasswordHash = PasswordHelper.Hash(newPassword);
        user.UpdatedDate = DateTime.UtcNow;

        await _users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    // ── Private Helpers ──────────────────────────────────────────

    private async Task EnsureUniqueUserAsync(string username, string email, int? userId = null)
    {
        username = username.Trim();
        email = email.Trim();

        if (await _users.AnyAsync(u => u.Username == username && (!userId.HasValue || u.Id != userId.Value)))
            throw new InvalidOperationException(Messages.User.UsernameAlreadyExists);

        if (await _users.AnyAsync(u => u.Email == email && (!userId.HasValue || u.Id != userId.Value)))
            throw new InvalidOperationException(Messages.User.EmailAlreadyExists);
    }

    private async Task ReplaceUserRolesAsync(int userId, IEnumerable<int> roleIds)
    {
        var existingRoles = await _userRoles.Query()
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

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
