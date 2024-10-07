namespace TaskManagement.Common.Middleware;

public interface IDomainEvent
{
    DateTime OccuredOn { get; set; }
}
public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken token = default);
}