namespace TaskManagement.Api.Cache.Redis;

internal class RedisSettings
{
    public int CacheExpirationInMinutes { get; set; } = 10;
}