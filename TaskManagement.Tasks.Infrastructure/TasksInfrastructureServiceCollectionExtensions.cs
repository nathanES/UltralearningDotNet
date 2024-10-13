using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Common.Interfaces;
using TaskManagement.Tasks.Infrastructure.Database;
using TaskManagement.Tasks.Infrastructure.Repositories;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Infrastructure;


public static class TasksInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddTasksInfrastructure(this IServiceCollection services, ConfigurationManager config)
    {
        //Useless now that we use EF to initialize Database
        // services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));

        services.AddDbContext<TasksContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("TasksDatabase")! + "TrustServerCertificate=True;");
        }, ServiceLifetime.Scoped);
        
        services.AddScoped<IDbInitializer, DbInitializer>();
        services.AddScoped<ITaskRepository, PostgresTaskRepository>();
        return services;
    }
}