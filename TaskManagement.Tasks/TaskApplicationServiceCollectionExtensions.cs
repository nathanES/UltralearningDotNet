using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Tasks.Services;
using TaskManagement.Tasks.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.Common;
using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Commands.DeleteTask;
using TaskManagement.Tasks.Commands.GetAllTasks;
using TaskManagement.Tasks.Commands.GetTask;
using TaskManagement.Tasks.Commands.UpdateTask;
using TaskManagement.Tasks.Interfaces;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks;

public static class TaskApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddTaskApplication(this IServiceCollection services)
    {
        services.TryAddCommonServices();
        services.AddScoped<ITaskService, TaskService>();

        services.AddScoped<IRequestHandler<CreateTaskCommand, bool>, CreateTaskHandler>();
        services.AddScoped<IRequestHandler<DeleteTaskCommand, bool>, DeleteTaskHandler>();
        services.AddScoped<IRequestHandler<GetAllTasksCommand, IEnumerable<Task>>, GetAllTasksHandler>();
        services.AddScoped<IRequestHandler<UpdateTaskCommand, Task?>, UpdateTaskHandler>();
        services.AddScoped<IRequestHandler<GetTaskCommand, Task?>, GetTaskHandler>();

        services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Singleton);
        return services;
    }
}