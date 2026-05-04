using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Baselib.Business.DTOs;
using Baselib.Business.Interfaces;
using Baselib.Data;
using Baselib.Entities;
using Microsoft.Extensions.Configuration;

namespace Baselib.Business.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.IsActive)
            .ToListAsync();

        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new Exception("Username already exists");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Email already exists");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            DepartmentId = dto.DepartmentId,
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        if (dto.RoleIds.Any())
        {
            foreach (var roleId in dto.RoleIds)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }
            await _context.SaveChangesAsync();
        }

        return (await GetByIdAsync(user.Id))!;
    }

    public async Task UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new Exception("User not found");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Phone = dto.Phone;
        user.DepartmentId = dto.DepartmentId;
        user.IsActive = dto.IsActive;
        user.UpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new Exception("User not found");

        user.IsActive = false;
        user.UpdatedDate = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid username or password");

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var existingTokens = _context.RefreshTokens.Where(rt => rt.UserId == user.Id);
        _context.RefreshTokens.RemoveRange(existingTokens);

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiryDate = DateTime.Now.AddDays(7)
        });
        await _context.SaveChangesAsync();

        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiryDate = DateTime.Now.AddMinutes(15),
            User = MapToDto(user)
        };
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .Include(rt => rt.User)
                .ThenInclude(u => u.Department)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.Now);

        if (token == null) throw new Exception("Invalid refresh token");

        var user = token.User;
        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        token.Token = newRefreshToken;
        token.ExpiryDate = DateTime.Now.AddDays(7);
        await _context.SaveChangesAsync();

        return new AuthResultDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiryDate = DateTime.Now.AddMinutes(15),
            User = MapToDto(user)
        };
    }

    public async Task LogoutAsync(int userId)
    {
        var tokens = _context.RefreshTokens.Where(rt => rt.UserId == userId);
        _context.RefreshTokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }

    public async Task AssignRolesAsync(int userId, List<int> roleIds)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        var existingRoles = _context.UserRoles.Where(ur => ur.UserId == userId);
        _context.UserRoles.RemoveRange(existingRoles);

        foreach (var roleId in roleIds)
        {
            _context.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        }
        await _context.SaveChangesAsync();
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

        var roles = _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToList();

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
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
            IsActive = user.IsActive,
            CreatedDate = user.CreatedDate
        };
    }
}