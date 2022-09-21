using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Apprenticeships.Domain
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Send<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent
        {
            var handlers = _serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();

            foreach (var handler in handlers)
            {
                await handler.Handle(@event, cancellationToken);
            }
        }
    }
}
