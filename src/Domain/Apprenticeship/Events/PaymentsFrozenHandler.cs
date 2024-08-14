using NServiceBus;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class PaymentsFrozenHandler : IDomainEventHandler<PaymentsFrozen>
{
    private readonly IMessageSession _messageSession;

    public PaymentsFrozenHandler(IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    public async Task Handle(PaymentsFrozen @event, CancellationToken cancellationToken = default(CancellationToken))
    {
        var paymentsFrozenEvent = new PaymentsFrozenEvent
        {
            ApprenticeshipKey = @event.ApprenticeshipKey
        };

        await _messageSession.Publish(paymentsFrozenEvent);
    }
}