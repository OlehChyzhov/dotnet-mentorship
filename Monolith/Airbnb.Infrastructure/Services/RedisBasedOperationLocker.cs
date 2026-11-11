using Airbnb.Application.Abstractions.Services;
using StackExchange.Redis;

namespace Airbnb.Infrastructure.Services;

public class RedisBasedOperationLocker : ICacheBasedOperationLocker
{
    private readonly IConnectionMultiplexer _redis;

    public RedisBasedOperationLocker(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<bool> TryLock(string key, string lockToken, TimeSpan expiration)
    {
        IDatabase db = _redis.GetDatabase();
        return await db.LockTakeAsync(key, lockToken, expiration);
    }

    public async Task<bool> ReleaseAsync(string key, string lockToken)
    {
        return await _redis.GetDatabase().LockReleaseAsync(key, lockToken);
    }
}