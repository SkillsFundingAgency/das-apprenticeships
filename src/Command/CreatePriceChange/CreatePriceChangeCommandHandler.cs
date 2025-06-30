using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Extensions;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.Infrastructure.Services;
using SFA.DAS.Learning.Types;

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
            var learning = await _learningRepository.Get(command.LearningKey);
            var now = _systemClockService.UtcNow.DateTime;

            if (!Enum.TryParse(command.Initiator, out ChangeInitiator initiator))
                throw new ArgumentException("CreateApprenticeshipPriceChangeRequest should have a valid initiator value set (Provider or Employer)", nameof(command));
            

            if (initiator == ChangeInitiator.Provider)
            {
                learning.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, now, ChangeRequestStatus.Created, command.UserId, command.Reason, null, now, null, initiator);
            }
            else
            {
                learning.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, now, ChangeRequestStatus.Created, null, command.Reason, command.UserId, null, now, initiator);
            }

            await _learningRepository.Update(learning);

            if (initiator == ChangeInitiator.Provider && IsNewTotalPriceLessThanExisting(learning, command))
            {
	            var priceChange = learning.ProviderAutoApprovePriceChange();
	            returnStatus = ChangeRequestStatus.Approved;
	            await _learningRepository.Update(learning);
                await SendEvent(learning, priceChange);
            }

            return returnStatus;
        }

        private static bool IsNewTotalPriceLessThanExisting(LearningDomainModel learningDomainModel, CreatePriceChangeCommand command)
        {
            return learningDomainModel.LatestPrice != null && (learningDomainModel.LatestPrice.TotalPrice >= command.TotalPrice);
        }

        public async Task SendEvent(LearningDomainModel learning, PriceHistoryDomainModel priceChange)
        {
            _logger.LogInformation("Sending provider autoapproved LearningPriceChangedEvent for apprenticeship {apprenticeshipKey}", learning.Key);

            var eventMessage = new LearningPriceChangedEvent()
            {
                LearningKey = learning.Key,
                LearningId = learning.ApprovalsApprenticeshipId,
                EffectiveFromDate = priceChange.EffectiveFromDate,
                ApprovedDate = priceChange.ProviderApprovedDate!.Value,
                ApprovedBy = ApprovedBy.Provider,
                Episode = learning.BuildEpisodeForIntegrationEvent()
            };

            await _messageSession.Publish(eventMessage);
        }
    }
}