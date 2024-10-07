using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Common.Interfaces;
using TaskManagement.Tasks.Infrastructure.Database;
using TaskManagement.Tasks.Infrastructure.Repositories;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Infrastructure;


public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddTaskInfrastructure(this IServiceCollection services, string connectionString)
    {
        //Useless now that we use EF to initialize Database
        // services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));

        services.AddDbContext<TasksContext>(options =>
        {
            options.UseNpgsql(connectionString + "TrustServerCertificate=True;"+ "Include Error Detail=true;");
        }, ServiceLifetime.Scoped);
        
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<ITaskRepository, PostgresTaskRepository>();
        return services;
    }
}