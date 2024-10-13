namespace TaskManagement.Api.Cache.OutputCache;

public static class OutputCacheExtensions
{
    public static IServiceCollection AddOutputCacheServices(this IServiceCollection services)
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
                    .SetVaryByQuery(new[]
                        { "title", "description", "deadline", "priority", "status", "page", "pageSize" })
                    .Tag(PolicyConstants.GetAllTasksCache.tag));
            options.AddPolicy(PolicyConstants.GetTaskCache.name, policy =>
                policy.Cache()
                    .Expire(TimeSpan.FromMinutes(2))
                    .SetVaryByQuery(new[] { "id" })
                    .Tag(PolicyConstants.GetTaskCache.name));

            options.AddPolicy(PolicyConstants.GetAllUsersCache.name, policy =>
                policy.Cache()
                    .Expire(TimeSpan.FromMinutes(2))
                    .SetVaryByQuery(new[] { "username", "email" })
                    .Tag(PolicyConstants.GetAllUsersCache.tag));
            options.AddPolicy(PolicyConstants.GetUserCache.name, policy =>
                policy.Cache()
                    .Expire(TimeSpan.FromMinutes(2))
                    .SetVaryByQuery(new[] { "id" })
                    .Tag(PolicyConstants.GetUserCache.tag));
        }); //Is an outputCache ServerSide
        return services;
    }

    public static IApplicationBuilder AddOutputCacheApplications(this IApplicationBuilder app)
    {
        app.UseOutputCache();
        return app;
    } 
}