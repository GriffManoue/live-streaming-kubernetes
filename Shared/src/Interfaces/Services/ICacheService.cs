using System;
using System.Threading.Tasks;

namespace Shared.Interfaces;

public interface ICacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);
    Task SetAddAsync(string key, string value);
    Task SetRemoveAsync(string key, string value);
    Task<int> SetCountAsync(string key);
    Task ExpireAsync(string key, TimeSpan expiry);
}
