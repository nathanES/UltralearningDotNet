using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Users.Infrastructure.Database;
using TaskManagement.Users.Infrastructure.Repositories;
using TaskManagement.Users.Interfaces;

namespace TaskManagement.Users.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {

        services.AddDbContext<UsersContext>(options =>
        {
            options.UseNpgsql(connectionString + "TrustServerCertificate=True;");
        }, ServiceLifetime.Scoped);
        
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        return services;
    }
}