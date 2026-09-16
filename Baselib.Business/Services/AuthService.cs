using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using static Baselib.Core.Constants.Constants.Jwt;
using Baselib.Core.Interfaces;
using Baselib.Core.Messages;
using Baselib.Core.Results;
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

    public async Task<IDataResult<AuthResultDto>> LoginAsync(LoginDto dto)
    {
        var identifier = dto.Username?.Trim() ?? string.Empty;
        var user = await _users.FirstOrDefaultAsync(
            u => u.Username == identifier || u.Email == identifier,
            include: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(u => u.Department));

        if (user == null || !PasswordHelper.Verify(dto.Password, user.PasswordHash))
            return DataResult<AuthResultDto>.Unauthorized(Messages.User.InvalidCredentials);

        var activeRole = user.UserRoles.FirstOrDefault()?.Role;

        var accessToken = GenerateToken(user, activeRole?.Id);
        var refreshToken = JwtHelper.GenerateRefreshToken();

        await ReplaceRefreshTokenAsync(user.Id, refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return DataResult<AuthResultDto>.Ok(BuildAuthResult(accessToken, refreshToken, user, activeRole?.Id));
    }

    public async Task<IDataResult<AuthResultDto>> RefreshTokenAsync(string refreshToken)
    {
        var token = await _refreshTokens.FirstOrDefaultAsync(
            rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.UtcNow,
            include: q => q.Include(rt => rt.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                           .Include(rt => rt.User).ThenInclude(u => u.Department));

        if (token == null)
            return DataResult<AuthResultDto>.Unauthorized(Messages.Auth.InvalidRefreshToken);

        var user = token.User;
        var activeRole = user.UserRoles.FirstOrDefault()?.Role;

        var newAccessToken = GenerateToken(user, activeRole?.Id);
        var newRefreshToken = JwtHelper.GenerateRefreshToken();

        token.ExpiryDate = DateTime.UtcNow; // Eski token'ı geçersiz kıl
        await ReplaceRefreshTokenAsync(user.Id, newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return DataResult<AuthResultDto>.Ok(BuildAuthResult(newAccessToken, newRefreshToken, user, activeRole?.Id));
    }

    public async Task<IResult> LogoutAsync(System.Security.Claims.ClaimsPrincipal principal)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);

        var tokens = await _refreshTokens.GetAllAsync(rt => rt.UserId == userId);

        _refreshTokens.RemoveRange(tokens);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.Auth.LoggedOut);
    }

    public async Task<IDataResult<AuthResultDto>> SwitchRoleAsync(System.Security.Claims.ClaimsPrincipal principal, int newRoleId)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);

        var user = await _users.FirstOrDefaultAsync(
            u => u.Id == userId,
            include: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(u => u.Department));

        if (user == null)
            return DataResult<AuthResultDto>.NotFound(Messages.User.NotFound);

        if (!user.UserRoles.Any(ur => ur.RoleId == newRoleId))
            return DataResult<AuthResultDto>.Unauthorized(Messages.Role.NoSwitchAccess);

        var accessToken = GenerateToken(user, newRoleId);
        var refreshToken = JwtHelper.GenerateRefreshToken();

        await ReplaceRefreshTokenAsync(user.Id, refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return DataResult<AuthResultDto>.Ok(BuildAuthResult(accessToken, refreshToken, user, newRoleId));
    }

    // ── Private Helpers ──────────────────────────────────────────

    private string GenerateToken(User user, int? activeRoleId)
    {
        return JwtHelper.GenerateAccessToken(
            user, activeRoleId,
            _configuration[Key]!,
            _configuration[Issuer]!,
            _configuration[Audience]!,
            AccessTokenExpiryMinutes);
    }

    private async Task ReplaceRefreshTokenAsync(int userId, string newToken)
    {
        var existingTokens = await _refreshTokens.GetAllAsync(rt => rt.UserId == userId);

        _refreshTokens.RemoveRange(existingTokens);

        await _refreshTokens.AddAsync(new RefreshToken
        {
            UserId = userId,
            Token = newToken,
            ExpiryDate = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays)
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
            ExpiryDate = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes),
            User = userDto
        };
    }
}
