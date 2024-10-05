using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Tasks.Services;
using TaskManagement.Tasks.Validators;
using FluentValidation;

namespace TaskManagement.Tasks;

public static class TaskApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddTaskApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Singleton);
        return services;
    }
}