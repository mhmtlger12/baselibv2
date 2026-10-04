using Baselib.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Baselib.Data.Repositories;

public sealed class RefreshTokenStore(AppDbContext context) : IRefreshTokenStore
{
    public async Task<bool> TryConsumeAsync(int tokenId, DateTime now)
    {
        var affected = await context.RefreshTokens.IgnoreQueryFilters()
            .Where(token => token.Id == tokenId && token.RevokedDate == null && token.ExpiryDate > now)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(token => token.RevokedDate, now)
                .SetProperty(token => token.LastUsedDate, now)
                .SetProperty(token => token.RevokedReason, "Rotated"));
        return affected == 1;
    }
}
