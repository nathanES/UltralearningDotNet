using TaskManagement.Api.Features.Tasks;
using TaskManagement.Api.Features.Users;

namespace TaskManagement.Api.Features;

public static class FeatureExtension
{
    public static IServiceCollection AddFeatureServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddTaskFeatureServices(config);
        services.AddUserFeatureServices(config);
        return services;
    }  
}