using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;

namespace TaskManagement.Api.Cache.Redis;

internal class RedisCacheService(IDistributedCache cache,
    ILogger<RedisCacheService> logger,
    IOptions<RedisSettings> redisSettings,
    IConnectionMultiplexer redisConnection) : ICacheService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value; 

    public async Task<Result<string>> GetFromCache(string cacheKey, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            logger.LogWarning("CacheKey is null or empty. Skipping Get From Cache.");
            return Result<string>.Failure(new ValidationError("CacheKey is null or empty", nameof(cacheKey))); 
        }
        
        try
        {
            var cachedValue = await cache.GetStringAsync(cacheKey, token);
            if (string.IsNullOrWhiteSpace(cachedValue))
            {
                logger.LogInformation("CachedValue is null or empty");
                return Result<string>.Failure(new NotFoundError("Cache"));
            }
            return Result<string>.Success(cachedValue);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to Get from cache {CacheKey}", cacheKey);
            await InvalidateCacheWithCacheKey(cacheKey, token); //Invalidate cache if data is corrupt or anything
            return Result<string>.Failure(new GenericError($"Failed to Get from cache : {cacheKey}", e.Message));
        }
    }
    public async Task<Result<None>> AddToCache(string cacheKey, string response, string tag, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            logger.LogWarning("CacheKey is null or empty. Skipping cache insertion.");
            return Result<None>.Failure(new ValidationError("CacheKey is null or empty", nameof(cacheKey)));
        }
        if (string.IsNullOrWhiteSpace(response))
        {
            logger.LogWarning("Response is null or empty. Skipping cache insertion.");
            return Result<None>.Failure(new ValidationError("Response is null or empty", nameof(response)));
        }
        
        try
        {
            await cache.SetStringAsync(cacheKey, response, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_redisSettings.CacheExpirationInMinutes)
            }, token);
            
            if (!string.IsNullOrWhiteSpace(tag))
            {
                var db = redisConnection.GetDatabase();
                await db.SetAddAsync($"tag:{tag}", cacheKey);
            }
            return Result<None>.Success(None.Value);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add to cache {CacheKey}", cacheKey);
            return Result<None>.Failure(new GenericError($"Failed to add to cache : {cacheKey}", e.Message));
        }
    }
 
    public async Task<Result<None>> InvalidateCacheWithCacheKey(string cacheKey, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            logger.LogWarning("CacheKey is null or empty. Skipping cacheKey-based cache invalidation.");
            return Result<None>.Failure(new ValidationError("CacheKey is null or empty", nameof(cacheKey)));
        }
        
        try
        {
            await cache.RemoveAsync(cacheKey, token);
            return Result<None>.Success(None.Value);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to invalidate the cache with CacheKey {CacheKey}", cacheKey);
            return Result<None>.Failure(new GenericError($"Failed to invalidate the cache : {cacheKey}", e.Message));
        }
    }
    public async Task<Result<None>> InvalidateCacheWithPattern(string pattern, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            logger.LogWarning("Pattern is null or empty. Skipping pattern-based cache invalidation.");
            return Result<None>.Failure(new ValidationError("Pattern is null or empty", nameof(pattern)));
        }

        try
        {
            var endPoints = redisConnection.GetEndPoints();
            foreach (var endpoint in endPoints)
            {
                var server = redisConnection.GetServer(endpoint);
                var keys = server.Keys(pattern: pattern);

                foreach (var key in keys)
                {
                    await cache.RemoveAsync(key!, token);
                }
            }
            return Result<None>.Success(None.Value);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to invalidate the cache with Pattern {Pattern}", pattern);
            return Result<None>.Failure(new GenericError($"Failed to invalidate the cache with Pattern {pattern}", e.Message));
        }
    }
    public async Task<Result<None>> InvalidateCacheWithTag(string tag, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            logger.LogWarning("Tag is null or empty. Skipping tag-based cache invalidation.");
            return Result<None>.Failure(new ValidationError("Tag is null or empty", nameof(tag)));
        }

        try
        {
            var db = redisConnection.GetDatabase();
            var keys = await db.SetMembersAsync($"tag:{tag}");

            foreach (var key in keys)
            {
                await cache.RemoveAsync(key.ToString(), token);
            }

            await db.KeyDeleteAsync($"tag:{tag}");
            return Result<None>.Success(None.Value);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to invalidate cache with Tag {Tag}", tag);
            return Result<None>.Failure(new GenericError($"Failed to invalidate cache with Tag  {tag}", e.Message));
        }
    }
}