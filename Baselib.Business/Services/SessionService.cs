using Baselib.Business.Interfaces;
using Baselib.Core.Interfaces;
using Baselib.Entities;

namespace Baselib.Business.Services;

public sealed class SessionService(
    IRepository<UserSession> sessions,
    IRepository<RefreshToken> refreshTokens,
    IRepository<UserRole> userRoles,
    TimeProvider timeProvider) : ISessionService
{
    public async Task<bool> IsActiveAsync(int userId, string familyId, int? activeRoleId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(familyId) || !await sessions.AnyAsync(session =>
                session.UserId == userId && session.FamilyId == familyId && session.RevokedDate == null && session.User.IsActive && !session.User.IsDeleted,
                ignoreQueryFilters: true, cancellationToken: cancellationToken)) return false;

        return !activeRoleId.HasValue || await userRoles.AnyAsync(ur =>
            ur.UserId == userId && ur.RoleId == activeRoleId.Value && ur.User.IsActive && ur.Role.IsActive,
            cancellationToken: cancellationToken);
    }

    public async Task<string> CreateAsync(int userId, CancellationToken cancellationToken = default)
    {
        var familyId = Guid.NewGuid().ToString("N");
        await sessions.AddAsync(new UserSession
        {
            FamilyId = familyId, UserId = userId, CreatedDate = timeProvider.GetUtcNow().UtcDateTime
        }, cancellationToken: cancellationToken);
        return familyId;
    }

    public Task RevokeAsync(int userId, string familyId, string reason, CancellationToken cancellationToken = default) => RevokeMatchingAsync(userId, reason, familyId, cancellationToken: cancellationToken);
    public Task RevokeAllAsync(int userId, string reason, CancellationToken cancellationToken = default) => RevokeMatchingAsync(userId, reason, familyId: null, cancellationToken: cancellationToken);

    private async Task RevokeMatchingAsync(int userId, string reason, string? familyId, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var activeSessions = await sessions.GetAllAsync(session => session.UserId == userId &&
            (familyId == null || session.FamilyId == familyId) && session.RevokedDate == null, ignoreQueryFilters: true, cancellationToken: cancellationToken);
        foreach (var session in activeSessions)
        {
            session.RevokedDate = now;
            session.RevokedReason = reason;
        }

        var activeTokens = await refreshTokens.GetAllAsync(token => token.UserId == userId &&
            (familyId == null || token.FamilyId == familyId) && token.RevokedDate == null, ignoreQueryFilters: true, cancellationToken: cancellationToken);
        foreach (var token in activeTokens)
        {
            token.RevokedDate = now;
            token.RevokedReason = reason;
        }
    }
}
