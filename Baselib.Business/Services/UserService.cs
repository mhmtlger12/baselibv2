using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Baselib.Business.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<UserRole> _userRoles;
    private readonly IRepository<RefreshToken> _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public UserService(
        IRepository<User> users,
        IRepository<UserRole> userRoles,
        IRepository<RefreshToken> refreshTokens,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _users = users;
        _userRoles = userRoles;
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _users.Query()
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.IsActive)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();

        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _users.Query()
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        await EnsureUniqueUserAsync(dto.Username, dto.Email);

        var user = new User
        {
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
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
            throw new KeyNotFoundException("User not found");

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
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _users.UpdateAsync(user);
        await ReplaceUserRolesAsync(id, dto.RoleIds);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsActive = false;
        user.UpdatedDate = DateTime.UtcNow;
        await _users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto)
    {
        var user = await _users.Query()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsActive);

        if (user == null || !IsPasswordValid(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password");

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var existingTokens = await _refreshTokens.Query()
            .Where(rt => rt.UserId == user.Id)
            .ToListAsync();

        _refreshTokens.RemoveRange(existingTokens);

        await _refreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        });

        await _unitOfWork.SaveChangesAsync();

        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddMinutes(15),
            User = MapToDto(user)
        };
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
    {
        var token = await _refreshTokens.Query()
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .Include(rt => rt.User)
                .ThenInclude(u => u.Department)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.UtcNow);

        if (token == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var user = token.User;
        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        token.Token = newRefreshToken;
        token.ExpiryDate = DateTime.UtcNow.AddDays(7);
        await _refreshTokens.UpdateAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResultDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiryDate = DateTime.UtcNow.AddMinutes(15),
            User = MapToDto(user)
        };
    }

    public async Task LogoutAsync(int userId)
    {
        var tokens = await _refreshTokens.Query()
            .Where(rt => rt.UserId == userId)
            .ToListAsync();

        _refreshTokens.RemoveRange(tokens);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AssignRolesAsync(int userId, List<int> roleIds)
    {
        if (!await _users.AnyAsync(u => u.Id == userId))
            throw new KeyNotFoundException("User not found");

        await ReplaceUserRolesAsync(userId, roleIds);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureUniqueUserAsync(string username, string email, int? userId = null)
    {
        username = username.Trim();
        email = email.Trim();

        if (await _users.AnyAsync(u => u.Username == username && (!userId.HasValue || u.Id != userId.Value)))
            throw new InvalidOperationException("Username already exists");

        if (await _users.AnyAsync(u => u.Email == email && (!userId.HasValue || u.Id != userId.Value)))
            throw new InvalidOperationException("Email already exists");
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

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.UserRoles.Select(ur => ur.Role.Name).Distinct())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool IsPasswordValid(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.Department?.Name,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            RoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList(),
            IsActive = user.IsActive,
            CreatedDate = user.CreatedDate
        };
    }
}
