using NServiceBus;
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
            var approval = apprenticeship.Episodes.Single();
            var latestEpisode = apprenticeship.LatestEpisode;
            var latestPrice = apprenticeship.LatestPrice;
            var apprenticeshipCreatedEvent = new ApprenticeshipCreatedEvent
            {
                ApprenticeshipKey = apprenticeship.Key, 
                Uln = apprenticeship.Uln,
                TrainingCode = latestEpisode.TrainingCode,
                FundingEmployerAccountId = approval.FundingEmployerAccountId,
                AgreedPrice = latestPrice.TotalPrice,
                FundingType = (FundingType)approval.FundingType,
                ActualStartDate = apprenticeship.StartDate,
                ApprovalsApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
                EmployerAccountId = latestEpisode.EmployerAccountId,
                LegalEntityName = approval.LegalEntityName,
                PlannedEndDate = apprenticeship.EndDate,
                UKPRN = latestEpisode.Ukprn,
                FundingBandMaximum = latestPrice.FundingBandMaximum,
                DateOfBirth = apprenticeship.DateOfBirth,
                FirstName = apprenticeship.FirstName, 
                LastName = apprenticeship.LastName,
                AgeAtStartOfApprenticeship = apprenticeship.AgeAtStartOfApprenticeship,
                PlannedStartDate = apprenticeship.StartDate, //todo do we need both actual and planned start dates? if so, need to double check if this has been handled correctly (e.g. actual vs planned)
                FundingPlatform = (FundingPlatform?)approval.FundingPlatform
            };

            await _messageSession.Publish(apprenticeshipCreatedEvent);
        }
    }
}
