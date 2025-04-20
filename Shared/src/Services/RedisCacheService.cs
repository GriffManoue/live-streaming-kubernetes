using System;
using System.Text.Json;
using System.Threading.Tasks;
using Shared.Interfaces;
using StackExchange.Redis;

namespace Shared.Services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    
    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, serializedValue, expiry);
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }
        
        return JsonSerializer.Deserialize<T>(value!);
    }
    
    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task SetAddAsync(string key, string value)
    {
        await _db.SetAddAsync(key, value);
    }

    public async Task SetRemoveAsync(string key, string value)
    {
        await _db.SetRemoveAsync(key, value);
    }

    public async Task<int> SetCountAsync(string key)
    {
        var count = await _db.SetLengthAsync(key);
        return (int)count;
    }

    public async Task ExpireAsync(string key, TimeSpan expiry)
    {
        await _db.KeyExpireAsync(key, expiry);
    }
}
