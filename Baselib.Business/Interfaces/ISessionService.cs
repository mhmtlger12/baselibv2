namespace Baselib.Business.Interfaces;

public interface ISessionService
{
    Task<bool> IsActiveAsync(int userId, string familyId, int? activeRoleId, CancellationToken ct = default);
    // Creation and revocation are persisted by the enclosing unit of work.
    Task<string> CreateAsync(int userId);
    Task RevokeAsync(int userId, string familyId, string reason);
    Task RevokeAllAsync(int userId, string reason);
}
