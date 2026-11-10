namespace FunkoAppi01.Cache;

public interface IServiceCache
{
    Task<T?> GetAsync <T>(string key) where T : class;
    Task SetASync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    Task<bool> RemoveAsync(string key);
    Task ClearAsync();

}