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
    public async Task<bool> IsActiveAsync(int userId, string familyId, int? activeRoleId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(familyId) || !await sessions.AnyAsync(session =>
                session.UserId == userId && session.FamilyId == familyId && session.RevokedDate == null && session.User.IsActive,
                ignoreQueryFilters: true, cancellationToken: ct)) return false;

        return !activeRoleId.HasValue || await userRoles.AnyAsync(ur =>
            ur.UserId == userId && ur.RoleId == activeRoleId.Value && ur.User.IsActive && ur.Role.IsActive,
            cancellationToken: ct);
    }

    public async Task<string> CreateAsync(int userId)
    {
        var familyId = Guid.NewGuid().ToString("N");
        await sessions.AddAsync(new UserSession
        {
            FamilyId = familyId, UserId = userId, CreatedDate = timeProvider.GetUtcNow().UtcDateTime
        });
        return familyId;
    }

    public Task RevokeAsync(int userId, string familyId, string reason) => RevokeMatchingAsync(userId, reason, familyId);
    public Task RevokeAllAsync(int userId, string reason) => RevokeMatchingAsync(userId, reason, familyId: null);

    private async Task RevokeMatchingAsync(int userId, string reason, string? familyId)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var activeSessions = await sessions.GetAllAsync(session => session.UserId == userId &&
            (familyId == null || session.FamilyId == familyId) && session.RevokedDate == null, ignoreQueryFilters: true);
        foreach (var session in activeSessions)
        {
            session.RevokedDate = now;
            session.RevokedReason = reason;
        }

        var activeTokens = await refreshTokens.GetAllAsync(token => token.UserId == userId &&
            (familyId == null || token.FamilyId == familyId) && token.RevokedDate == null, ignoreQueryFilters: true);
        foreach (var token in activeTokens)
        {
            token.RevokedDate = now;
            token.RevokedReason = reason;
        }
    }
}
