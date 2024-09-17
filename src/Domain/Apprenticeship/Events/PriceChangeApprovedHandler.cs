using NServiceBus;
using SFA.DAS.Apprenticeships.Domain.Extensions;
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
        var priceChange = apprenticeship.PriceHistories.Single(x => x.Key == @event.PriceHistoryKey);
        var priceChangeApprovedEvent = new ApprenticeshipPriceChangedEvent()
        {
            ApprenticeshipKey = apprenticeship.Key, 
            ApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
            EffectiveFromDate = priceChange.EffectiveFromDate,
            ApprovedDate = @event.ApprovedBy == ApprovedBy.Employer ? priceChange.EmployerApprovedDate!.Value : priceChange.ProviderApprovedDate!.Value,
            ApprovedBy = @event.ApprovedBy,
            ProviderApprovedBy = priceChange.ProviderApprovedBy,
            EmployerApprovedBy = priceChange.EmployerApprovedBy,
            Episode = apprenticeship.BuildEpisodeForIntegrationEvent()
        };

        await _messageSession.Publish(priceChangeApprovedEvent);
    }
}