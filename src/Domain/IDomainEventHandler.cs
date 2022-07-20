namespace SFA.DAS.Apprenticeships.Domain
{
    public interface IDomainEventHandler<in T> where T: IDomainEvent
    {
        Task Handle(T @event, CancellationToken cancellationToken = default(CancellationToken));
    }
}
