using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.Common;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Middleware.PipelineBehaviors;
using TaskManagement.Tasks;
using TaskManagement.Tasks.Infrastructure;

namespace TaskManagement.Api.Features.Tasks;

public static class TaskFeatureExtension
{
    public static IServiceCollection AddTaskFeatureServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddTaskInfrastructure(config["Database:Task:ConnectionString"]!);
        services.AddTaskApplication();
        return services;
    } 
}