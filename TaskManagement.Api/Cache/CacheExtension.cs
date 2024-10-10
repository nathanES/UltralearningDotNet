namespace TaskManagement.Api.Cache;

public static class CacheExtension
{

    public static IServiceCollection AddCacheServices(this IServiceCollection services,
        ConfigurationManager config)
    {
        // services.AddMemoryCache(); 
        // services.AddResponseCaching(); //allows you to cache entire HTTP responses on the server and return cached responses for subsequent requests.
        services.AddOutputCache(options =>
        {
            // Global policy that caches by default
            options.AddBasePolicy(policy => policy.Cache());

            // Specific policy for tasks
            options.AddPolicy(PolicyConstants.GetAllTasksCache.name, policy =>
                policy.Cache()
                    .Expire(TimeSpan.FromMinutes(2))
                    .SetVaryByQuery(new[] { "title", "description", "deadline", "priority", "status", "page", "pageSize" })
                    .Tag(PolicyConstants.GetAllTasksCache.tag));
            options.AddPolicy(PolicyConstants.GetTaskCache.name, policy =>
                policy.Cache()
                    .Expire(TimeSpan.FromMinutes(2))
                    .SetVaryByQuery(new[] { "id" })
                    .Tag(PolicyConstants.GetTaskCache.name));
            
            options.AddPolicy(PolicyConstants.GetAllUsersCache.name, policy =>
                policy.Cache()
                    .Expire(TimeSpan.FromMinutes(2))
                    .SetVaryByQuery(new[] { "username", "email"})
                    .Tag(PolicyConstants.GetAllUsersCache.tag));
            options.AddPolicy(PolicyConstants.GetUserCache.name, policy =>
                policy.Cache()
                    .Expire(TimeSpan.FromMinutes(2))
                    .SetVaryByQuery(new[] { "id" })
                    .Tag(PolicyConstants.GetUserCache.tag));
            
        });
        return services;
    }

    public static IApplicationBuilder AddCacheApplication(this IApplicationBuilder app)
    {
        app.UseOutputCache();
        return app;
    }
    
}