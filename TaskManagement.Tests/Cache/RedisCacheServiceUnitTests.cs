using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using TaskManagement.Api.Cache.Redis;
using TaskManagement.Common.ResultPattern.Errors;

namespace TaskManagement.Tests.Cache;

public class RedisCacheServiceUnitTests
{
    private readonly ICacheService _cacheService;
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly Mock<IConnectionMultiplexer> _redisConnectionMock;
    private readonly Mock<ILogger<RedisCacheService>> _logger;
    private readonly Mock<IDatabase> _redisDatabaseMock;

    public RedisCacheServiceUnitTests()
    {
        _logger = new Mock<ILogger<RedisCacheService>>();

        var redisSettings = new RedisSettings() { CacheExpirationInMinutes = 10 };
        var configMock = new Mock<IOptions<RedisSettings>>();
        configMock.Setup(config => config.Value).Returns(redisSettings);

        _distributedCacheMock = new Mock<IDistributedCache>();

        _redisConnectionMock = new Mock<IConnectionMultiplexer>();
        _redisDatabaseMock = new Mock<IDatabase>();
        _redisConnectionMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_redisDatabaseMock.Object);

        _cacheService = new RedisCacheService(_distributedCacheMock.Object,
            _logger.Object, configMock.Object, _redisConnectionMock.Object);
    }

    #region AddToCache

    [Theory]
    [InlineData("TestKey", "TestResponse", "TestTag")]
    public async Task AddToCache_ShouldReturnSuccess_WhenCacheIsAdded(string cacheKey, string response, string tag)
    {
        //Arrange
        AddToCacheRedisConfigurationOk(tag, cacheKey);

        //Act
        var result = await _cacheService.AddToCache(cacheKey, response, tag, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.SetAsync(
                cacheKey,
                It.IsAny<byte[]>(), // Verify that the string was converted to bytes
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _redisDatabaseMock.Verify(x => x.SetAddAsync($"tag:{tag}", cacheKey, CommandFlags.None),
            Times.Once);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AddToCache_ShouldReturnValidationError_WhenCacheKeyIsInvalid(string cacheKey)
    {
        //Arrange
        var response = "TestResponse";
        string tag = "TestTag";
        AddToCacheRedisConfigurationOk(tag, cacheKey);

        //Act
        var result = await _cacheService.AddToCache(cacheKey, response, tag, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.SetAsync(
                cacheKey,
                It.IsAny<byte[]>(), // Verify that the string was converted to bytes
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _redisDatabaseMock.Verify(x => x.SetAddAsync($"tag:{tag}", cacheKey, CommandFlags.None),
            Times.Never);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AddToCache_ShouldReturnValidationError_WhenResponseIsInvalid(string response)
    {
        //Arrange
        string cacheKey = "TestKey";
        string tag = "TestTag";
        AddToCacheRedisConfigurationOk(tag, cacheKey);
        //Act
        var result = await _cacheService.AddToCache(cacheKey, response, tag, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.SetAsync(
                cacheKey,
                It.IsAny<byte[]>(), // Verify that the string was converted to bytes
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _redisDatabaseMock.Verify(x => x.SetAddAsync($"tag:{tag}", cacheKey, CommandFlags.None),
            Times.Never);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AddToCache_ShouldReturnSuccessButDontAddTagToDb_WhenTagIsInvalid(string tag)
    {
        //Arrange
        string cacheKey = "TestKey";
        string response = "TestResponse";
        AddToCacheRedisConfigurationOk(tag, cacheKey);

        //Act
        var result = await _cacheService.AddToCache(cacheKey, response, tag, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.SetAsync(
                cacheKey,
                It.IsAny<byte[]>(), // Verify that the string was converted to bytes
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _redisDatabaseMock.Verify(x => x.SetAddAsync($"tag:{tag}", cacheKey, CommandFlags.None),
            Times.Never);
        Assert.True(result.IsSuccess);
    }

    private void AddToCacheRedisConfigurationOk(string tag, string cacheKey)
    {
        _redisDatabaseMock.Setup(x => x.SetAddAsync($"tag:{tag}", cacheKey, CommandFlags.None))
            .ReturnsAsync(true);
        _distributedCacheMock
            .Setup(cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(), // Because SetStringAsync internally converts the string to bytes
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(Task.CompletedTask);
    }

    #endregion

    #region GetFromCache

    [Theory]
    [InlineData("testCacheKey")]
    public async Task GetFromCache_ShouldReturnSuccess_WhenCacheKeyIsValidAndCacheExist(string cacheKey)
    {
        //Arrange
        string cachedValue = "cachedValue";
        byte[] cachedValueBytes = System.Text.Encoding.UTF8.GetBytes(cachedValue); // Convert to byte array

        //GetStringAsync call GetAsync Internally so we mock GetAsync
        _distributedCacheMock
            .Setup(cache => cache.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(cachedValueBytes);
        //Act
        var result = await _cacheService.GetFromCache(cacheKey, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.GetAsync(
                cacheKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.True(result.IsSuccess);
        Assert.Equal(cachedValue, result.Response);
    }

    [Theory]
    [InlineData("")]
    // [InlineData(null)] //How to handle null value 
    public async Task GetFromCache_ShouldReturnNotFoundError_WhenCacheKeyIsValidAndCacheDontExist(string cachedValue)
    {
        //Arrange
        string cacheKey = "testCacheKey";
        byte[] cachedValueBytes = System.Text.Encoding.UTF8.GetBytes(cachedValue); // Convert to byte array

        //GetStringAsync call GetAsync Internally so we mock GetAsync
        _distributedCacheMock
            .Setup(cache => cache.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(cachedValueBytes);
        //Act
        var result = await _cacheService.GetFromCache(cacheKey, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.GetAsync(
                cacheKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is NotFoundError);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetFromCache_ShouldReturnValidationError_WhenCacheKeyIsNotValid(string cacheKey)
    {
        //Arrange
        string cachedValue = "cachedValue";
        byte[] cachedValueBytes = System.Text.Encoding.UTF8.GetBytes(cachedValue); // Convert to byte array

        //GetStringAsync call GetAsync Internally so we mock GetAsync
        _distributedCacheMock
            .Setup(cache => cache.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(cachedValueBytes);
        //Act
        var result = await _cacheService.GetFromCache(cacheKey, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.GetAsync(
                cacheKey,
                It.IsAny<CancellationToken>()),
            Times.Never);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    #endregion

    #region InvalidateCacheWithCacheKey

    [Theory]
    [InlineData("testCacheKey")]
    public async Task InvalidateCacheWithCacheKey_ShouldReturnSuccess_WhenCacheKeyIsValid(string cacheKey)
    {
        //Arrange
        _distributedCacheMock
            .Setup(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(Task.CompletedTask);
        //Act
        var result = await _cacheService.InvalidateCacheWithCacheKey(cacheKey, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.RemoveAsync(
                cacheKey,
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task InvalidateCacheWithCacheKey_ShouldReturnValidationError_WhenCacheKeyIsInvalid(string cacheKey)
    {
        //Arrange
        _distributedCacheMock
            .Setup(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(Task.CompletedTask);
        //Act
        var result = await _cacheService.InvalidateCacheWithCacheKey(cacheKey, default);

        //Assert
        _distributedCacheMock.Verify(cache => cache.RemoveAsync(
                cacheKey,
                It.IsAny<CancellationToken>()),
            Times.Never);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    #endregion

    #region InvalidateCacheWithPattern

    [Theory]
    [InlineData("testCacheKey*")]
    public async Task InvalidateCacheWithPattern_ShouldReturnSuccess_WhenPatternIsValidAndCacheKeyExist(string pattern)
    {
        // Arrange
        var redisServerMock = new Mock<IServer>();
        var endPoints = new System.Net.EndPoint[]
            { new System.Net.DnsEndPoint("localhost", 6379) };
        var redisKeys = new RedisKey[] { "testCacheKey1", "testCacheKey2" };

        _redisConnectionMock.Setup(x => x.GetEndPoints(It.IsAny<bool>()))
            .Returns(endPoints);

        // Mocking RedisServer.Keys to return the Redis keys
        redisServerMock.Setup(server => server.Keys(
                It.IsAny<int>(),
                It.IsAny<RedisValue>(),
                It.IsAny<int>(),
                It.IsAny<long>(),
                It.IsAny<int>(),
                CommandFlags.None))
            .Returns(redisKeys);

        _redisConnectionMock.Setup(x => x.GetServer(It.IsAny<System.Net.EndPoint>(), It.IsAny<object>()))
            .Returns(redisServerMock.Object);

        _distributedCacheMock.Setup(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cacheService.InvalidateCacheWithPattern(pattern, default);

        // Assert: 
        foreach (var key in redisKeys)
        {
            _distributedCacheMock.Verify(cache => cache.RemoveAsync(
                    key,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("testCacheKey*")]
    public async Task InvalidateCacheWithPattern_ShouldReturnSuccess_WhenPatternIsValidAndCacheDontExist(string pattern)
    {
        // Arrange
        var redisServerMock = new Mock<IServer>();
        var endPoints = new System.Net.EndPoint[]
            { new System.Net.DnsEndPoint("localhost", 6379) };

        _redisConnectionMock.Setup(x => x.GetEndPoints(It.IsAny<bool>()))
            .Returns(endPoints);

        // Mocking RedisServer.Keys to return the Redis keys
        redisServerMock.Setup(server => server.Keys(
                It.IsAny<int>(),
                It.IsAny<RedisValue>(),
                It.IsAny<int>(),
                It.IsAny<long>(),
                It.IsAny<int>(),
                CommandFlags.None))
            .Returns(Array.Empty<RedisKey>());

        _redisConnectionMock.Setup(x => x.GetServer(It.IsAny<System.Net.EndPoint>(), It.IsAny<object>()))
            .Returns(redisServerMock.Object);

        _distributedCacheMock.Setup(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cacheService.InvalidateCacheWithPattern(pattern, default);

        // Assert: 
        _distributedCacheMock.Verify(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task InvalidateCacheWithPattern_ShouldReturnValidationError_WhenPatternIsInvalid(string pattern)
    {
        // Arrange

        // Act
        var result = await _cacheService.InvalidateCacheWithPattern(pattern, default);

        // Assert: 
        _distributedCacheMock.Verify(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }

    #endregion

    #region InvalidateCacheWithTag

    [Theory]
    [InlineData("tag")]
    public async Task InvalidateCacheWithTag_ShouldReturnSuccess_WhenTagIsValidAndCacheExist(string tag)
    {
        // Arrange
        var redisValues = new RedisValue[] { new RedisValue("hello") { }, new RedisValue("dd") { } };
        _redisDatabaseMock.Setup(x => x.SetMembersAsync($"tag:{tag}", CommandFlags.None))
            .ReturnsAsync(redisValues);

        _distributedCacheMock.Setup(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cacheService.InvalidateCacheWithTag(tag, default);

        // Assert: 
        foreach (var redisValue in redisValues)
        {
            _distributedCacheMock.Verify(cache => cache.RemoveAsync(
                    redisValue!,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("tag")]
    public async Task InvalidateCacheWithTag_ShouldReturnSuccess_WhenTagIsValidAndCacheDontExist(string tag)
    {
        // Arrange
        _redisDatabaseMock.Setup(x => x.SetMembersAsync($"tag:{tag}", CommandFlags.None))
            .ReturnsAsync(Array.Empty<RedisValue>());

        _distributedCacheMock.Setup(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cacheService.InvalidateCacheWithTag(tag, default);

        // Assert: 
        _distributedCacheMock.Verify(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        
        Assert.True(result.IsSuccess);
    }
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task InvalidateCacheWithTag_ShouldReturnValidationError_WhenTagIsInvalid(string tag)
    {
        // Arrange
        // Act
        var result = await _cacheService.InvalidateCacheWithTag(tag, default);

        // Assert: 
        _distributedCacheMock.Verify(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e is ValidationError);
    }
    #endregion
}