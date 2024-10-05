namespace TaskManagement.Common.Mediator;

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token = default);
}
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();