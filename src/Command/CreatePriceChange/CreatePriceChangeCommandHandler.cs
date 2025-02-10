using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprenticeships.Command.ApprovePriceChange;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Extensions;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Command.CreatePriceChange
{
    public class CreatePriceChangeCommandHandler : ICommandHandler<CreatePriceChangeCommand, ChangeRequestStatus>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly IMessageSession _messageSession;
        private readonly ISystemClockService _systemClockService;
        private readonly ILogger<CreatePriceChangeCommandHandler> _logger;

        public CreatePriceChangeCommandHandler(
            IApprenticeshipRepository apprenticeshipRepository,
            IMessageSession messageSession,
            ISystemClockService systemClockService,
            ILogger<CreatePriceChangeCommandHandler> logger)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
            _messageSession = messageSession;
            _systemClockService = systemClockService;
            _logger = logger;
        }

        public async Task<ChangeRequestStatus> Handle(CreatePriceChangeCommand command,
            CancellationToken cancellationToken = default)
        {
            var returnStatus = ChangeRequestStatus.Created;
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            var now = _systemClockService.UtcNow.DateTime;

            if (!Enum.TryParse(command.Initiator, out ChangeInitiator initiator))
                throw new ArgumentException("CreateApprenticeshipPriceChangeRequest should have a valid initiator value set (Provider or Employer)", nameof(command));
            

            if (initiator == ChangeInitiator.Provider)
            {
                apprenticeship.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, now, ChangeRequestStatus.Created, command.UserId, command.Reason, null, now, null, initiator);
            }
            else
            {
                apprenticeship.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, now, ChangeRequestStatus.Created, null, command.Reason, command.UserId, null, now, initiator);
            }

            await _apprenticeshipRepository.Update(apprenticeship);

            if (initiator == ChangeInitiator.Provider && IsNewTotalPriceLessThanExisting(apprenticeship, command))
            {
	            var priceChange = apprenticeship.ProviderAutoApprovePriceChange();
	            returnStatus = ChangeRequestStatus.Approved;
	            await _apprenticeshipRepository.Update(apprenticeship);
                await SendEvent(apprenticeship, priceChange);
            }

            return returnStatus;
        }

        private static bool IsNewTotalPriceLessThanExisting(ApprenticeshipDomainModel apprenticeshipDomainModel, CreatePriceChangeCommand command)
        {
            return apprenticeshipDomainModel.LatestPrice != null && (apprenticeshipDomainModel.LatestPrice.TotalPrice >= command.TotalPrice);
        }

        public async Task SendEvent(ApprenticeshipDomainModel apprenticeship, PriceHistoryDomainModel priceChange)
        {
            _logger.LogInformation("Sending provider autoapproved ApprenticeshipPriceChangedEvent for apprenticeship {apprenticeshipKey}", apprenticeship.Key);

            var eventMessage = new ApprenticeshipPriceChangedEvent()
            {
                ApprenticeshipKey = apprenticeship.Key,
                ApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
                EffectiveFromDate = priceChange.EffectiveFromDate,
                ApprovedDate = priceChange.ProviderApprovedDate!.Value,
                ApprovedBy = ApprovedBy.Provider,
                Episode = apprenticeship.BuildEpisodeForIntegrationEvent()
            };

            await _messageSession.Publish(eventMessage);
        }
    }
}