using NServiceBus;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class PriceChangeApprovedHandler : IDomainEventHandler<PriceChangeApproved>
{
    private readonly IApprenticeshipRepository _repository;
    private readonly IMessageSession _messageSession;

    public PriceChangeApprovedHandler(IApprenticeshipRepository repository, IMessageSession messageSession)
    {
        _repository = repository;
        _messageSession = messageSession;
    }

    public async Task Handle(PriceChangeApproved @event, CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _repository.Get(@event.ApprenticeshipKey);
        var episode = apprenticeship.LatestEpisode;
        var priceChange = apprenticeship.PriceHistories.Single(x => x.Key == @event.PriceHistoryKey);
        var apprenticeshipCreatedEvent = new PriceChangeApprovedEvent
        {
            ApprenticeshipKey = apprenticeship.Key,
            ApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
            EmployerAccountId = episode.EmployerAccountId,
            ApprovedDate = @event.ApprovedBy == ApprovedBy.Employer ? priceChange.EmployerApprovedDate!.Value : priceChange.ProviderApprovedDate!.Value,
            ApprovedBy = @event.ApprovedBy,
            AssessmentPrice = priceChange.AssessmentPrice!.Value,
            TrainingPrice = priceChange.TrainingPrice!.Value,
            EffectiveFromDate = priceChange.EffectiveFromDate,
            ProviderId = episode.Ukprn,
            EpisodeKey = @event.AmendedPrices.EpisodeKey,
            PriceKey = @event.AmendedPrices.LatestPriceKey,
            DeletedPriceKeys = @event.AmendedPrices.DeletedPriceKeys
        };

        await _messageSession.Publish(apprenticeshipCreatedEvent);
    }
}