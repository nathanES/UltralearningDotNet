using TaskManagement.Users;
using TaskManagement.Users.Infrastructure;

namespace TaskManagement.Api.Features.Users;

public static class UsersExtensions
{
    public static IServiceCollection AddUsersServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddUsersInfrastructure(config);
        services.AddUserApplication();
        return services;
    } 
}