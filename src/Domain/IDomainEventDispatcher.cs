namespace SFA.DAS.Learning.Domain
{
    public interface IDomainEventDispatcher
    {
        Task Send<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken = default(CancellationToken)) where TDomainEvent : IDomainEvent;
    }
}
