using TaskManagement.Api.Features.JWT.Endpoints;
using TaskManagement.Api.Features.Tasks.Endpoints;
using TaskManagement.Api.Features.Users.Endpoints;

namespace TaskManagement.Api.Features;

public static class ApiEndpointsExtensions
{
    public static IEndpointRouteBuilder AddApiEndpoints(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        switch (config["CacheType"])
        {
            case "Redis":
                app.AddTasksEndpointsRedisCache(config);
                app.AddUsersEndpointsRedisCache(config);
                app.AddJwtEndpoints();
                break;
            case "OutputCache":
                app.AddTasksEndpointsOutputCache(config);
                app.AddUsersEndpointsOutputCache(config);
                app.AddJwtEndpoints();
                break;
            default:
                break;
        }

        
        return app;
    }
}