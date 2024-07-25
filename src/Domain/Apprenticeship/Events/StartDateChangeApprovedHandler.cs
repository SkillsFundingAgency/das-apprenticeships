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

    public async Task Handle(StartDateChangeApproved @event, CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _repository.Get(@event.ApprenticeshipKey);
        var startDateChange = apprenticeship.StartDateChanges.Single(x => x.Key == @event.StartDateChangeKey);
        var episode = apprenticeship.LatestEpisode;
        var startDateChangedEvent = new OldApprenticeshipStartDateChangedEvent()
        {
            ApprenticeshipKey = apprenticeship.Key,
            ApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
            EmployerAccountId = episode.EmployerAccountId,
            ApprovedDate = @event.ApprovedBy == ApprovedBy.Employer ? startDateChange.EmployerApprovedDate!.Value : startDateChange.ProviderApprovedDate!.Value,
            ProviderId = episode.Ukprn,
            ActualStartDate = startDateChange.ActualStartDate,
            PlannedEndDate = startDateChange.PlannedEndDate,
            AgeAtStartOfApprenticeship = apprenticeship.AgeAtStartOfApprenticeship,
            ProviderApprovedBy = startDateChange.ProviderApprovedBy,
            EmployerApprovedBy = startDateChange.EmployerApprovedBy,
            Initiator = startDateChange.Initiator.ToString()!,
            ApprenticeshipEpisodeKey = @event.AmendedPrices.ApprenticeshipEpisodeKey,
            PriceKey = @event.AmendedPrices.LatestEpisodePrice.GetEntity().Key,
            DeletedPriceKeys = @event.AmendedPrices.DeletedPriceKeys
        };

        await _messageSession.Publish(startDateChangedEvent);
    }
}