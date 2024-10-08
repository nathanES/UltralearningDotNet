using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Middleware.PipelineBehaviors;

namespace TaskManagement.Common;

public static class CommonServiceCollectionExtensions
{
    public static IServiceCollection TryAddCommonServices(this IServiceCollection services)
    {
        services.TryAddScoped<IMediator, Mediator>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UserExistenceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TaskExistenceBehavior<,>));

        return services;
    }
    public static IServiceCollection AddRequestHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        // Find all classes that implement IRequestHandler<TRequest, TResponse>
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

        // Register each handler type
        foreach (var handlerType in handlerTypes)
        {
            var handlerInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            services.AddScoped(handlerInterface, handlerType);
        }

        return services;
    }
}