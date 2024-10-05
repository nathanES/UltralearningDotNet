using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Tasks.Services;
using TaskManagement.Tasks.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.Common;
using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks;

public static class TaskApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddTaskApplication(this IServiceCollection services)
    {
        services.TryAddCommonServices();
        services.TryAddScoped<IRequestHandler<CreateTaskCommand, bool>, CreateTaskHandler>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Singleton);
        return services;
    }
}