using NServiceBus;
using SFA.DAS.Apprenticeships.Domain.Extensions;
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

    public async Task Handle(StartDateChangeApproved @event, CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _repository.Get(@event.ApprenticeshipKey);
        var startDateChange = apprenticeship.StartDateChanges.Single(x => x.Key == @event.StartDateChangeKey);
        var startDateChangedEvent = new ApprenticeshipStartDateChangedEvent()
        {
            ApprenticeshipKey = apprenticeship.Key, 
            ApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
            StartDate = apprenticeship.StartDate,
            ApprovedDate = @event.ApprovedBy == ApprovedBy.Employer ? startDateChange.EmployerApprovedDate!.Value : startDateChange.ProviderApprovedDate!.Value,
            ProviderApprovedBy = startDateChange.ProviderApprovedBy,
            EmployerApprovedBy = startDateChange.EmployerApprovedBy,
            Initiator = startDateChange.Initiator.ToString()!,
            Episode = apprenticeship.BuildEpisodeForIntegrationEvent()
        };
        
        await _messageSession.Publish(startDateChangedEvent);
    }
}