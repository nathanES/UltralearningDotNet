using StackExchange.Redis;

namespace TaskManagement.Api.Cache.Redis;

public static class RedisCacheExtensions
{
    public static IServiceCollection AddRedisCacheServices(this IServiceCollection services,
        ConfigurationManager config)
    {
        //Redis need to run, see the redis kubernetes file
        services.Configure<RedisSettings>(config.GetSection("Redis"));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.GetConnectionString("Redis");
            options.InstanceName = "TaskManagementRedisInstance";
        });
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(config.GetConnectionString("Redis")!, true);
            configuration.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(configuration);
        });
        
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        return services;
    } 
}