using TaskManagement.Common.ResultPattern;

namespace TaskManagement.Api.Cache.Redis;

internal interface ICacheService
{
    Task<Result<string>> GetFromCache(string cacheKey, CancellationToken token = default);
    Task<Result<None>> AddToCache(string cacheKey, string response, string tag, CancellationToken token = default);
    Task<Result<None>> InvalidateCacheWithCacheKey(string cacheKey, CancellationToken token = default);
    Task<Result<None>> InvalidateCacheWithPattern(string pattern, CancellationToken token = default);
    Task<Result<None>> InvalidateCacheWithTag(string tag, CancellationToken token = default);
}