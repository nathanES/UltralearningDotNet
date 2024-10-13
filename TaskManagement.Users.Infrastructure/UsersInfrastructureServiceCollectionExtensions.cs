using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Common.Interfaces;
using TaskManagement.Users.Infrastructure.Database;
using TaskManagement.Users.Infrastructure.Repositories;
using TaskManagement.Users.Interfaces;

namespace TaskManagement.Users.Infrastructure;

public static class UsersInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, ConfigurationManager config)
    {

        services.AddDbContext<UsersContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("UsersDatabase") + "TrustServerCertificate=True;");
        }, ServiceLifetime.Scoped);
        
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        return services;
    }
}