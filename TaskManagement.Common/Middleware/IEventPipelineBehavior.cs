namespace TaskManagement.Common.Middleware;

public interface IEventPipelineBehavior<TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, EventHandlerDelegate next, CancellationToken token);
}

public delegate Task EventHandlerDelegate();