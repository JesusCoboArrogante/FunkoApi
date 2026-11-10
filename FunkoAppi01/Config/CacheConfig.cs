namespace FunkoAppi01.Config;

public class CacheConfig
{
    public const string SectionName = "CacheSettings";
    public string Provider { get; set; } = "Memory";
    public string RedisConnectionString { get; set; } = "localhost:6379";
    public int ExpirationSeconds { get; set; } = 60;
}