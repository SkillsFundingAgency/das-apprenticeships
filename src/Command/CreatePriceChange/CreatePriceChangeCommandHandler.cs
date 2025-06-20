using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Extensions;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.Infrastructure.Services;
using SFA.DAS.Apprenticeship.Types;

namespace SFA.DAS.Learning.Command.CreatePriceChange
{
    public class CreatePriceChangeCommandHandler : ICommandHandler<CreatePriceChangeCommand, ChangeRequestStatus>
    {
        private readonly ILearningRepository _learningRepository;
        private readonly IMessageSession _messageSession;
        private readonly ISystemClockService _systemClockService;
        private readonly ILogger<CreatePriceChangeCommandHandler> _logger;

        public CreatePriceChangeCommandHandler(
            ILearningRepository learningRepository,
            IMessageSession messageSession,
            ISystemClockService systemClockService,
            ILogger<CreatePriceChangeCommandHandler> logger)
        {
            _learningRepository = learningRepository;
            _messageSession = messageSession;
            _systemClockService = systemClockService;
            _logger = logger;
        }

        public async Task<ChangeRequestStatus> Handle(CreatePriceChangeCommand command,
            CancellationToken cancellationToken = default)
        {
            var returnStatus = ChangeRequestStatus.Created;
            var apprenticeship = await _learningRepository.Get(command.LearningKey);
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

            await _learningRepository.Update(apprenticeship);

            if (initiator == ChangeInitiator.Provider && IsNewTotalPriceLessThanExisting(apprenticeship, command))
            {
	            var priceChange = apprenticeship.ProviderAutoApprovePriceChange();
	            returnStatus = ChangeRequestStatus.Approved;
	            await _learningRepository.Update(apprenticeship);
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