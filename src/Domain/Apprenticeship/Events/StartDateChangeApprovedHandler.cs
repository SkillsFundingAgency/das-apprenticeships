using NServiceBus;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class StartDateChangeApprovedHandler : IDomainEventHandler<StartDateChangeApproved>
{
    private readonly IApprenticeshipRepository _repository;
    private readonly IMessageSession _messageSession;

    public StartDateChangeApprovedHandler(IApprenticeshipRepository repository, IMessageSession messageSession)
    {
        _repository = repository;
        _messageSession = messageSession;
    }

    public async Task Handle(StartDateChangeApproved @event, CancellationToken cancellationToken = default(CancellationToken))
    {
        var apprenticeship = await _repository.Get(@event.ApprenticeshipKey);
        var approval = apprenticeship.Episodes.Single();
        var startDateChange = apprenticeship.StartDateChanges.Single(x => x.Key == @event.StartDateChangeKey);
        //todo amend handler for start date change
        //var startDateChangedEvent = new ApprenticeshipStartDateChangedEvent()
        //{
        //    ApprenticeshipKey = apprenticeship.Key,
        //    ApprenticeshipId = approval.ApprovalsApprenticeshipId,
        //    EmployerAccountId = apprenticeship.EmployerAccountId,
        //    ApprovedDate = @event.ApprovedBy == ApprovedBy.Employer ? startDateChange.EmployerApprovedDate!.Value : startDateChange.ProviderApprovedDate!.Value,
        //    ProviderId = apprenticeship.Ukprn,
        //    ActualStartDate = startDateChange.ActualStartDate,
        //    PlannedEndDate = startDateChange.PlannedEndDate,
        //    AgeAtStartOfApprenticeship = apprenticeship.AgeAtStartOfApprenticeship,
        //    ProviderApprovedBy = startDateChange.ProviderApprovedBy,
        //    EmployerApprovedBy = startDateChange.EmployerApprovedBy,
        //    Initiator = startDateChange.Initiator.ToString()!
        //};

        //await _messageSession.Publish(startDateChangedEvent);
    }
}