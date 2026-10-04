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
    private readonly ISessionService _sessions;
    private readonly IRefreshTokenStore _tokenStore;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly AutoMapper.IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public AuthService(
        IRepository<User> users,
        IRepository<RefreshToken> refreshTokens,
        IRepository<AppSetting> settings,
        ISessionService sessions,
        IRefreshTokenStore tokenStore,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        AutoMapper.IMapper mapper,
        TimeProvider timeProvider)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _settings = settings;
        _sessions = sessions;
        _tokenStore = tokenStore;
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

        var familyId = await _sessions.CreateAsync(user.Id);
        var accessToken = GenerateToken(user, activeRole?.Id, familyId);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, activeRole?.Id, familyId, clientSession, now);

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
        if (token.RevokedDate.HasValue)
            return await RejectReuseAsync(token);
        if (token.ExpiryDate <= now ||
            !await _sessions.IsActiveAsync(token.UserId, token.FamilyId, token.ActiveRoleId))
            return DataResult<AuthResultDto>.Unauthorized(Messages.Auth.InvalidRefreshToken);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Conditional UPDATE takes the database row lock. Exactly one request can consume it.
            if (!await _tokenStore.TryConsumeAsync(token.Id, now))
            {
                await _unitOfWork.RollbackTransactionAsync();
                return await RejectReuseAsync(token);
            }

            var newRefreshToken = await CreateRefreshTokenAsync(token.UserId, token.ActiveRoleId, token.FamilyId, clientSession, now);
            var newAccessToken = GenerateToken(token.User, token.ActiveRoleId, token.FamilyId);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return DataResult<AuthResultDto>.Ok(BuildAuthResult(newAccessToken, newRefreshToken, token.User, token.ActiveRoleId));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionSafelyAsync();
            throw;
        }
    }

    private async Task<IDataResult<AuthResultDto>> RejectReuseAsync(RefreshToken token)
    {
        // Persist session revocation, so a concurrent successor cannot revive the family.
        await _sessions.RevokeAsync(token.UserId, token.FamilyId, "ReuseDetected");
        await _unitOfWork.SaveChangesAsync();
        return DataResult<AuthResultDto>.Unauthorized(Messages.Auth.InvalidRefreshToken);
    }

    public async Task<IResult> LogoutAsync(System.Security.Claims.ClaimsPrincipal principal)
    {
        var userId = ClaimsPrincipalHelper.GetUserId(principal);

        await _sessions.RevokeAllAsync(userId, "Logout");
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

        var currentFamilyId = principal.FindFirst(SessionIdClaim)?.Value;
        if (string.IsNullOrWhiteSpace(currentFamilyId) ||
            !await _sessions.IsActiveAsync(userId, currentFamilyId, ClaimsPrincipalHelper.GetActiveRoleId(principal)))
            return DataResult<AuthResultDto>.Unauthorized(Messages.Auth.InvalidRefreshToken);

        await _sessions.RevokeAsync(userId, currentFamilyId, "RoleChanged");
        var familyId = await _sessions.CreateAsync(user.Id);
        var accessToken = GenerateToken(user, newRoleId, familyId);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, newRoleId, familyId, clientSession, _timeProvider.GetUtcNow().UtcDateTime);

        await _unitOfWork.SaveChangesAsync();

        return DataResult<AuthResultDto>.Ok(BuildAuthResult(accessToken, refreshToken, user, newRoleId));
    }

    // ── Private Helpers ──────────────────────────────────────────

    private string GenerateToken(User user, int? activeRoleId, string familyId)
    {
        return JwtHelper.GenerateAccessToken(
            user, activeRoleId, familyId,
            _configuration[Key]!,
            _configuration[Issuer]!,
            _configuration[Audience]!,
            _timeProvider.GetUtcNow(),
            AccessTokenExpiryMinutes);
    }

    private async Task<string> CreateRefreshTokenAsync(
        int userId,
        int? activeRoleId,
        string familyId,
        ClientSessionInfoDto clientSession,
        DateTime now)
    {
        var refreshToken = JwtHelper.GenerateRefreshToken();
        await _refreshTokens.AddAsync(new RefreshToken
        {
            UserId = userId,
            TokenHash = JwtHelper.HashRefreshToken(refreshToken),
            FamilyId = familyId,
            ActiveRoleId = activeRoleId,
            ExpiryDate = now.AddDays(RefreshTokenExpiryDays),
            CreatedDate = now,
            IpAddress = Limit(clientSession.IpAddress, 45),
            UserAgent = Limit(clientSession.UserAgent, 512)
        });

        return refreshToken;
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
