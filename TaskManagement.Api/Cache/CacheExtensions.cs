using StackExchange.Redis;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;

namespace TaskManagement.Api.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services,
        ConfigurationManager config)
    {
        switch (config["CacheType"])
        {
            case "Redis":   
                services.AddRedisCacheServices(config);
                break;
            case "Output":
                services.AddOutputCacheServices();
                break;
            default:
                break;
        }
        return services;
    }

    public static IApplicationBuilder AddCacheApplications(this IApplicationBuilder app, 
        ConfigurationManager config)
    {
        switch (config["CacheType"])
        {
            case "Redis":   
                break;
            case "Output":
                app.AddOutputCacheApplications();
                break;
            default:
                break;
        }
        return app;
    }

    

   
}