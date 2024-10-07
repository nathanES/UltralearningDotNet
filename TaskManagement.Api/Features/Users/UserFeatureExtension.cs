using TaskManagement.Users;
using TaskManagement.Users.Infrastructure;

namespace TaskManagement.Api.Features.Users;

public static class UserFeatureExtension
{
    public static IServiceCollection AddUserFeatureServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddUserInfrastructure(config["Database:User:ConnectionString"]!);
        services.AddUserApplication();
        return services;
    } 
}