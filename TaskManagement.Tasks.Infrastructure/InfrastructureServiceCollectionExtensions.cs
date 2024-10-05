using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Tasks.Database;
using TaskManagement.Tasks.Infrastructure.Database;
using TaskManagement.Tasks.Infrastructure.Repositories;
using TaskManagement.Tasks.Repositories;

namespace TaskManagement.Tasks.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        //Useless now that we use EF to initialize Database
        // services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));

        services.AddDbContext<TasksContext>(options =>
        {
            options.UseNpgsql(connectionString + "TrustServerCertificate=True;");
        }, ServiceLifetime.Scoped);
        
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<ITaskRepository, PostgreTaskRepository>();
        return services;
    }
}