using TaskManagement.Api.Features.JWT;
using TaskManagement.Api.Features.Tasks;
using TaskManagement.Api.Features.Users;

namespace TaskManagement.Api.Features;

public static class FeaturesExtensions
{
    public static IServiceCollection AddFeaturesServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddTasksServices(config);
        services.AddUsersServices(config);
        services.AddJwtServices(config);
        return services;
    }  
}