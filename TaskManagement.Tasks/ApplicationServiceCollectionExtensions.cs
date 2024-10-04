using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Tasks.Services;
using TaskManagement.Tasks.Validators;
using FluentValidation;

namespace TaskManagement.Tasks;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Singleton);
        return services;
    }
}