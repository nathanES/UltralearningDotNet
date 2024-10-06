using Microsoft.Extensions.DependencyInjection;

namespace TaskManagement.Common.Mediator;

public class Mediator(IServiceProvider serviceProvider)
{
    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken token = default) where TRequest : IRequest<TResponse>
    {
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().ToList();
        var handler = serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();
        if (handler is null)
        {
            throw new InvalidOperationException($"No handler found for {typeof(TRequest).Name}");
        }

        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.HandleAsync(request, token);
        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.Handle(request, next, token);
        }

        return await handlerDelegate();
    }
}