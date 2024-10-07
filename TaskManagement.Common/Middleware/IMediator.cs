namespace TaskManagement.Common.Middleware;

public interface IMediator
{
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken token = default)
        where TRequest : IRequest<TResponse>;

    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken token = default)
        where TDomainEvent : IDomainEvent;

}