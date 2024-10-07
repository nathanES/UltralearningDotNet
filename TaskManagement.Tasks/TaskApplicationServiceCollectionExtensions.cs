using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Tasks.Services;
using TaskManagement.Tasks.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.Common;
using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Commands;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Commands.DeleteTask;
using TaskManagement.Tasks.Commands.GetAllTasks;
using TaskManagement.Tasks.Commands.GetTask;
using TaskManagement.Tasks.Commands.UpdateTask;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks;

public static class TaskApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddTaskApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddRequestHandlersFromAssembly(typeof(ICommandMarker).Assembly);
        // services.AddScoped<IRequestHandler<CreateTaskCommand, Result<Task>>, CreateTaskHandler>();
        // services.AddScoped<IRequestHandler<DeleteTaskCommand, Result<None>>, DeleteTaskHandler>();
        // services.AddScoped<IRequestHandler<GetAllTasksCommand, Result<IEnumerable<Task>>>, GetAllTasksHandler>();
        // services.AddScoped<IRequestHandler<UpdateTaskCommand, Result<Task>>, UpdateTaskHandler>();
        // services.AddScoped<IRequestHandler<GetTaskCommand, Result<Task>>, GetTaskHandler>();

        services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Transient);
        return services;
    }
}