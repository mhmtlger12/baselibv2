using Baselib.Business.DTOs;
using Baselib.Business.Helpers;
using Baselib.Business.Interfaces;
using static Baselib.Core.Constants.Constants.Jwt;
using Baselib.Core.Constants;
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
    private readonly IRepository<AppSetting> _settings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly AutoMapper.IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public AuthService(
        IRepository<User> users,
        IRepository<RefreshToken> refreshTokens,
        IRepository<AppSetting> settings,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        AutoMapper.IMapper mapper,
        TimeProvider timeProvider)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _settings = settings;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<IDataResult<AuthResultDto>> LoginAsync(LoginDto dto, ClientSessionInfoDto clientSession)
    {
        var identifier = UserIdentityHelper.Normalize(dto.Username);
        var user = await _users.FirstOrDefaultAsync(
            u => u.NormalizedUsername == identifier || u.NormalizedEmail == identifier,
            include: q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(u => u.Department));

        if (user == null)
            return DataResult<AuthResultDto>.Unauthorized(Messages.User.InvalidCredentials);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        if (user.LockoutEndDate.HasValue && user.LockoutEndDate > now)
            return DataResult<AuthResultDto>.Unauthorized(Messages.User.InvalidCredentials);

        if (user.LockoutEndDate.HasValue)
        {
            user.FailedLoginCount = 0;
            user.LockoutEndDate = null;
        }

        if (!PasswordHelper.Verify(dto.Password, user.PasswordHash))
        {
            user.FailedLoginCount++;
            if (user.FailedLoginCount >= await GetMaxLoginAttemptsAsync())
                user.LockoutEndDate = now.AddMinutes(Constants.Authentication.LockoutMinutes);

            _users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return DataResult<AuthResultDto>.Unauthorized(Messages.User.InvalidCredentials);
        }

        user.FailedLoginCount = 0;
        user.LockoutEndDate = null;

        var activeRole = user.UserRoles.FirstOrDefault()?.Role;

        var accessToken = GenerateToken(user, activeRole?.Id);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, activeRole?.Id, clientSession);

        await _unitOfWork.SaveChangesAsync();

        return DataResult<AuthResultDto>.Ok(BuildAuthResult(accessToken, refreshToken, user, activeRole?.Id));
    }

    public async Task<IDataResult<AuthResultDto>> RefreshTokenAsync(string refreshToken, ClientSessionInfoDto clientSession)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return DataResult<AuthResultDto>.Unauthorized(Messages.Auth.InvalidRefreshToken);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var tokenHash = JwtHelper.HashRefreshToken(refreshToken);
        var token = await _refreshTokens.FirstOrDefaultAsync(
            rt => rt.TokenHash == tokenHash,
            include: q => q.Include(rt => rt.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                           .Include(rt => rt.User).ThenInclude(u => u.Department));

        if (token == null)
            return DataResult<AuthResultDto>.Unauthorized(Messages.Auth.InvalidRefreshToken);

        // Döndürülmüş veya logout edilmiş tokenın yeniden kullanılması token hırsızlığı göstergesidir.
        // Aynı cihaz oturumundaki tüm tokenlar iptal edilir.
        if (token.RevokedDate.HasValue)
        {
            await RevokeFamilyAsync(token.FamilyId, now, "ReuseDetected");
            await _unitOfWork.SaveChangesAsync();
            return DataResult<AuthResultDto>.Unauthorized(Messages.Auth.InvalidRefreshToken);
        }

        if (token.ExpiryDate <= now)
            return DataResult<AuthResultDto>.Unauthorized(Messages.Auth.InvalidRefreshToken);

        var user = token.User;
        var activeRole = token.ActiveRoleId.HasValue
            ? user.UserRoles.FirstOrDefault(ur => ur.RoleId == token.ActiveRoleId.Value)?.Role
            : user.UserRoles.FirstOrDefault()?.Role;

        var newAccessToken = GenerateToken(user, activeRole?.Id);
        var newRefreshToken = await RotateRefreshTokenAsync(token, activeRole?.Id, clientSession, now);

        await _unitOfWork.SaveChangesAsync();

        return DataResult<AuthResultDto>.Ok(BuildAuthResult(newAccessToken, newRefreshToken, user, activeRole?.Id));
    }

    public async Task<IResult> LogoutAsync(System.Security.Claims.ClaimsPrincipal principal)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);

        await RevokeUserSessionsAsync(userId, _timeProvider.GetUtcNow().UtcDateTime, "Logout");
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(Messages.Auth.LoggedOut);
    }

    public async Task<IDataResult<AuthResultDto>> SwitchRoleAsync(
        System.Security.Claims.ClaimsPrincipal principal,
        int newRoleId,
        ClientSessionInfoDto clientSession)
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
        var refreshToken = await CreateRefreshTokenAsync(user.Id, newRoleId, clientSession);

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
            _timeProvider.GetUtcNow(),
            AccessTokenExpiryMinutes);
    }

    private async Task<string> CreateRefreshTokenAsync(
        int userId,
        int? activeRoleId,
        ClientSessionInfoDto clientSession)
    {
        var refreshToken = JwtHelper.GenerateRefreshToken();
        await _refreshTokens.AddAsync(new RefreshToken
        {
            UserId = userId,
            TokenHash = JwtHelper.HashRefreshToken(refreshToken),
            FamilyId = Guid.NewGuid().ToString("N"),
            ActiveRoleId = activeRoleId,
            ExpiryDate = _timeProvider.GetUtcNow().UtcDateTime.AddDays(RefreshTokenExpiryDays),
            CreatedDate = _timeProvider.GetUtcNow().UtcDateTime,
            IpAddress = Limit(clientSession.IpAddress, 45),
            UserAgent = Limit(clientSession.UserAgent, 512)
        });

        return refreshToken;
    }

    private async Task<string> RotateRefreshTokenAsync(
        RefreshToken currentToken,
        int? activeRoleId,
        ClientSessionInfoDto clientSession,
        DateTime now)
    {
        currentToken.RevokedDate = now;
        currentToken.RevokedReason = "Rotated";
        currentToken.LastUsedDate = now;

        var refreshToken = JwtHelper.GenerateRefreshToken();
        await _refreshTokens.AddAsync(new RefreshToken
        {
            UserId = currentToken.UserId,
            TokenHash = JwtHelper.HashRefreshToken(refreshToken),
            FamilyId = currentToken.FamilyId,
            ActiveRoleId = activeRoleId,
            ExpiryDate = now.AddDays(RefreshTokenExpiryDays),
            CreatedDate = now,
            IpAddress = Limit(clientSession.IpAddress, 45),
            UserAgent = Limit(clientSession.UserAgent, 512)
        });

        return refreshToken;
    }

    private async Task RevokeUserSessionsAsync(int userId, DateTime now, string reason)
    {
        var activeTokens = await _refreshTokens.GetAllAsync(
            rt => rt.UserId == userId && !rt.RevokedDate.HasValue);

        foreach (var activeToken in activeTokens)
        {
            activeToken.RevokedDate = now;
            activeToken.RevokedReason = reason;
        }
    }

    private async Task RevokeFamilyAsync(string familyId, DateTime now, string reason)
    {
        var activeTokens = await _refreshTokens.GetAllAsync(
            rt => rt.FamilyId == familyId && !rt.RevokedDate.HasValue);

        foreach (var activeToken in activeTokens)
        {
            activeToken.RevokedDate = now;
            activeToken.RevokedReason = reason;
        }
    }

    private static string? Limit(string? value, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Length <= maximumLength ? value : value[..maximumLength];
    }

    private async Task<int> GetMaxLoginAttemptsAsync()
    {
        var setting = await _settings.FirstOrDefaultAsync(setting => setting.Key == "MaxLoginAttempts");
        return int.TryParse(setting?.Value, out var maxAttempts) && maxAttempts is >= 3 and <= 20
            ? maxAttempts
            : Constants.Authentication.DefaultMaxLoginAttempts;
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
            ExpiryDate = _timeProvider.GetUtcNow().UtcDateTime.AddMinutes(AccessTokenExpiryMinutes),
            User = userDto
        };
    }
}
