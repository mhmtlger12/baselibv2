namespace Baselib.Core.Interfaces;

public interface IRefreshTokenStore
{
    // Runs within the caller's transaction, together with insertion of the successor token.
    Task<bool> TryConsumeAsync(int tokenId, DateTime now);
}
