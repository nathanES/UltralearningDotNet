using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.Common;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Middleware.PipelineBehaviors;
using TaskManagement.Tasks;
using TaskManagement.Tasks.Infrastructure;

namespace TaskManagement.Api.Features.Tasks;

public static class TasksExtensions
{
    public static IServiceCollection AddTasksServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddTasksInfrastructure(config);
        services.AddTasksApplication();
        return services;
    } 
}