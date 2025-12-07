namespace PhysicsProject.Core.Abstractions;

public interface IDomainEvent { }

public interface IDomainEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct) where TEvent : IDomainEvent;
}





