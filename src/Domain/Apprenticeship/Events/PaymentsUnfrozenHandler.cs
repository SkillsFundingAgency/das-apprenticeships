using NServiceBus;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class PaymentsUnfrozenHandler : IDomainEventHandler<PaymentsUnfrozen>
{
    private readonly IMessageSession _messageSession;

    public PaymentsUnfrozenHandler(IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    public async Task Handle(PaymentsUnfrozen @event, CancellationToken cancellationToken = default(CancellationToken))
    {
        var paymentsUnfrozenEvent = new PaymentsUnfrozenEvent
        {
            ApprenticeshipKey = @event.ApprenticeshipKey
        };

        await _messageSession.Publish(paymentsUnfrozenEvent);
    }
}