using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Common.Middleware;

internal class Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger) : IMediator
{
    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken token = default) where TRequest : IRequest<TResponse>
    {
        var handler = serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();
        if (handler is null)
        {
            throw new InvalidOperationException($"No handler found for {typeof(TRequest).Name}");
        }
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().ToList();
        logger.LogInformation($"Found {behaviors.Count} behaviors in the pipeline.");

        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.HandleAsync(request, token);
        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.HandleAsync(request, next, token);
        }

        return await handlerDelegate();
    }

    public async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken token = default) 
        where TDomainEvent : IDomainEvent
    {
        var handlers = serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();
        if (handlers == null || !handlers.Any())
        {
            logger.LogWarning($"No handlers found for domain event {typeof(TDomainEvent).Name}");
            return;    
        }
        logger.LogInformation($"Publishing event {typeof(TDomainEvent).Name} to {handlers.Count()} handlers");
        foreach (var handler in handlers)
        {
            try
            {
                logger.LogInformation($"Handling event {typeof(TDomainEvent).Name} with {handler.GetType().Name}");
                await handler.HandleAsync(domainEvent, token);
                logger.LogInformation($"Successfully handled event {typeof(TDomainEvent).Name} with {handler.GetType().Name}");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error while sending event {typeof(TDomainEvent).Name} to {handler.GetType().Name}");
            }
        }
    }
}