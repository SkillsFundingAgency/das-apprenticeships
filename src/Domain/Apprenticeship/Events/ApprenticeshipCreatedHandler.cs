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
            var approval = apprenticeship.Approvals.Single();
            var apprenticeshipCreatedEvent = new ApprenticeshipCreatedEvent
            {
                ApprenticeshipKey = apprenticeship.Key, 
                Uln = apprenticeship.Uln,
                TrainingCode = apprenticeship.TrainingCode,
                FundingEmployerAccountId = approval.FundingEmployerAccountId,
                AgreedPrice = approval.AgreedPrice,
                FundingType = (FundingType)approval.FundingType,
                ActualStartDate = approval.ActualStartDate,
                ApprovalsApprenticeshipId = approval.ApprovalsApprenticeshipId,
                EmployerAccountId = approval.EmployerAccountId,
                LegalEntityName = approval.LegalEntityName,
                PlannedEndDate = approval.PlannedEndDate,
                UKPRN = approval.UKPRN,
                FundingBandMaximum = approval.FundingBandMaximum,
                DateOfBirth = apprenticeship.DateOfBirth,
                FirstName = apprenticeship.FirstName, 
                LastName = apprenticeship.LastName,
                AgeAtStartOfApprenticeship = apprenticeship.,
                PlannedStartDate = approval.PlannedStartDate,
                FundingPlatform = (FundingPlatform?)approval.FundingPlatform
            };

            await _messageSession.Publish(apprenticeshipCreatedEvent);
        }
    }
}
