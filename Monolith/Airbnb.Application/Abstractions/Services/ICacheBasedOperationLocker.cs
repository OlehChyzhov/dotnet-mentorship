namespace Airbnb.Application.Abstractions.Services;

public interface ICacheBasedOperationLocker
{
    Task<bool> LockAsync(string key, string lockToken, TimeSpan expiration);
    Task<bool> ReleaseAsync(string key, string lockToken);
}