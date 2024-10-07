using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.Common.Middleware;

namespace TaskManagement.Common;

public static class CommonServiceCollectionExtensions
{
    public static IServiceCollection TryAddCommonServices(this IServiceCollection services)
    {
        services.TryAddScoped<IMediator, Mediator>();
        services.TryAddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }
}