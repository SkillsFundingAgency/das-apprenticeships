using NServiceBus;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class WithdrawnEventHandler : IDomainEventHandler<WithdrawnEvent>
{
    private readonly IMessageSession _messageSession;

    public WithdrawnEventHandler(IApprenticeshipRepository repository, IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    public async Task Handle(WithdrawnEvent @event, CancellationToken cancellationToken = default)
    {
        var message = new ApprenticeshipWithdrawnEvent
        {
            ApprenticeshipKey = @event.ApprenticeshipKey,
            ApprenticeshipId = @event.ApprenticeshipId,
            Reason = @event.Reason,
            LastDayOfLearning = @event.LastDayOfLearning
        };

        await _messageSession.Publish(message);
    }
}