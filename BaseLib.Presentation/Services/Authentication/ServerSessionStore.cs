using Baselib.Business.DTOs;
using Microsoft.Extensions.Caching.Memory;
namespace BaseLib.Presentation.Services.Authentication;

public sealed class ServerSession(AuthResultDto authentication)
{
    public AuthResultDto Authentication { get; set; } = authentication;
    public SemaphoreSlim Gate { get; } = new(1, 1);
}

// Swap this store for a distributed implementation when running multiple instances.
public interface IServerSessionStore
{
    string Create(AuthResultDto authentication);
    ServerSession? Find(string? id);
    void Remove(string? id);
}
public sealed class ServerSessionStore(IMemoryCache cache) : IServerSessionStore
{
    public string Create(AuthResultDto authentication)
    {
        var id = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        cache.Set("auth:" + id, new ServerSession(authentication), TimeSpan.FromHours(8));
        return id;
    }
    public ServerSession? Find(string? id) => id is null ? null : cache.Get<ServerSession>("auth:" + id);
    public void Remove(string? id) { if (id is not null) cache.Remove("auth:" + id); }
}
