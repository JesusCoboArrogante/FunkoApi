using System.Text.Json;
using FunkoAppi01.Config;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FunkoAppi01.Cache;

public class RedisCacheService (IConnectionMultiplexer redis, IOptions<CacheConfig> options): IServiceCache
{
    
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromSeconds(options.Value.ExpirationSeconds);
    
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var value = await _db.StringGetAsync(key);

        if (value.IsNullOrEmpty)
        {
            return null;
        }
        
        return JsonSerializer.Deserialize<T>((string)value);
    }

    public async Task SetASync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiration ?? _defaultExpiration);
    }

    public Task<bool> RemoveAsync(string key)
    {
        return _db.KeyDeleteAsync(key);
    }

    public async Task ClearAsync()
    {
        foreach (var endPoint in redis.GetEndPoints())
        {
            var server = redis.GetServer(endPoint);
            await server.FlushDatabaseAsync();
        }
    }
}