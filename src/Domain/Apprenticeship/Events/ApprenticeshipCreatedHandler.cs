using NServiceBus;
using SFA.DAS.Apprenticeships.Domain.Extensions;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events
{
    public class ApprenticeshipCreatedHandler : IDomainEventHandler<ApprenticeshipCreated>
    {
        private readonly IApprenticeshipRepository _repository;
        private readonly IMessageSession _messageSession;

        public ApprenticeshipCreatedHandler(IApprenticeshipRepository repository, IMessageSession messageSession)
        {
            _repository = repository;
            _messageSession = messageSession;
        }

        public async Task Handle(ApprenticeshipCreated @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            var apprenticeship = await _repository.Get(@event.ApprenticeshipKey);
            var apprenticeshipCreatedEvent = new ApprenticeshipCreatedEvent
            {
                ApprenticeshipKey = apprenticeship.Key, 
                Uln = apprenticeship.Uln,
                ApprovalsApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
                DateOfBirth = apprenticeship.DateOfBirth,
                FirstName = apprenticeship.FirstName, 
                LastName = apprenticeship.LastName,
                Episode = apprenticeship.BuildEpisodeForIntegrationEvent()
            };

            await _messageSession.Publish(apprenticeshipCreatedEvent);
        }
    }
}
