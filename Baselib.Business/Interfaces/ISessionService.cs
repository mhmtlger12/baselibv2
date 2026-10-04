namespace Baselib.Business.Interfaces;

public interface ISessionService
{
    Task<bool> IsActiveAsync(int userId, string familyId, int? activeRoleId, CancellationToken cancellationToken = default);
    // Creation and revocation are persisted by the enclosing unit of work.
    Task<string> CreateAsync(int userId, CancellationToken cancellationToken = default);
    Task RevokeAsync(int userId, string familyId, string reason, CancellationToken cancellationToken = default);
    Task RevokeAllAsync(int userId, string reason, CancellationToken cancellationToken = default);
}
