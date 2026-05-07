using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Data.Interfaces;
using Baselib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Baselib.Business.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<RefreshToken> _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly AutoMapper.IMapper _mapper;

    public AuthService(
        IRepository<User> users,
        IRepository<RefreshToken> refreshTokens,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        AutoMapper.IMapper mapper)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto)
    {
        var user = await _users.Query()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null || !PasswordHelper.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException(Messages.User.InvalidCredentials);

        var activeRole = user.UserRoles.FirstOrDefault()?.Role;

        var accessToken = GenerateToken(user, activeRole?.Id);
        var refreshToken = JwtHelper.GenerateRefreshToken();

        await ReplaceRefreshTokenAsync(user.Id, refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return BuildAuthResult(accessToken, refreshToken, user, activeRole?.Id);
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
            throw new UnauthorizedAccessException(Messages.Auth.InvalidRefreshToken);

        var user = token.User;
        var activeRole = user.UserRoles.FirstOrDefault()?.Role;

        var newAccessToken = GenerateToken(user, activeRole?.Id);
        var newRefreshToken = JwtHelper.GenerateRefreshToken();

        token.Token = newRefreshToken;
        token.ExpiryDate = DateTime.UtcNow.AddDays(7);
        await _refreshTokens.UpdateAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return BuildAuthResult(newAccessToken, newRefreshToken, user, activeRole?.Id);
    }

    public async Task LogoutAsync(System.Security.Claims.ClaimsPrincipal principal)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);

        var tokens = await _refreshTokens.Query()
            .Where(rt => rt.UserId == userId)
            .ToListAsync();

        _refreshTokens.RemoveRange(tokens);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<AuthResultDto> SwitchRoleAsync(System.Security.Claims.ClaimsPrincipal principal, int newRoleId)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);

        var user = await _users.Query()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException(Messages.User.NotFound);

        if (!user.UserRoles.Any(ur => ur.RoleId == newRoleId))
            throw new UnauthorizedAccessException(Messages.Role.NoSwitchAccess);

        var accessToken = GenerateToken(user, newRoleId);
        var refreshToken = JwtHelper.GenerateRefreshToken();

        await ReplaceRefreshTokenAsync(user.Id, refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return BuildAuthResult(accessToken, refreshToken, user, newRoleId);
    }

    // ── Private Helpers ──────────────────────────────────────────

    private string GenerateToken(User user, int? activeRoleId)
    {
        return JwtHelper.GenerateAccessToken(
            user, activeRoleId,
            _configuration["Jwt:Key"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!);
    }

    private async Task ReplaceRefreshTokenAsync(int userId, string newToken)
    {
        var existingTokens = await _refreshTokens.Query()
            .Where(rt => rt.UserId == userId)
            .ToListAsync();

        _refreshTokens.RemoveRange(existingTokens);

        await _refreshTokens.AddAsync(new RefreshToken
        {
            UserId = userId,
            Token = newToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        });
    }

    private AuthResultDto BuildAuthResult(string accessToken, string refreshToken, User user, int? activeRoleId)
    {
        var userDto = _mapper.Map<UserDto>(user);

        var activeRole = activeRoleId.HasValue
            ? user.UserRoles.FirstOrDefault(ur => ur.RoleId == activeRoleId.Value)?.Role
            : user.UserRoles.FirstOrDefault()?.Role;

        if (activeRole != null)
        {
            userDto.ActiveRoleId = activeRole.Id;
            userDto.ActiveRoleName = activeRole.Name;
        }

        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiryDate = DateTime.UtcNow.AddMinutes(15),
            User = userDto
        };
    }
}
