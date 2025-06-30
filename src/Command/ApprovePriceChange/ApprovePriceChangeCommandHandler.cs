using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Extensions;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.Infrastructure.Services;
using SFA.DAS.Learning.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;

namespace SFA.DAS.Learning.Command.ApprovePriceChange;

public class ApprovePriceChangeCommandHandler : ICommandHandler<ApprovePriceChangeCommand, ApprovedBy>
{
    private readonly ILearningRepository _learningRepository;
    private readonly IMessageSession _messageSession;
    private readonly ISystemClockService _systemClockService;
    private readonly ILogger<ApprovePriceChangeCommandHandler> _logger;

    public ApprovePriceChangeCommandHandler(
        ILearningRepository learningRepository,
        IMessageSession messageSession,
        ISystemClockService systemClockService,
        ILogger<ApprovePriceChangeCommandHandler> logger)
    {
        _learningRepository = learningRepository;
        _messageSession = messageSession;
        _systemClockService = systemClockService;
        _logger = logger;
    }

    public async Task<ApprovedBy> Handle(ApprovePriceChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Approving price change for learning {learningKey}", command.LearningKey);

        var learning = await _learningRepository.Get(command.LearningKey);
        var priceChange = learning.ApprovePriceChange(command.UserId, command.TrainingPrice, command.AssessmentPrice, _systemClockService.UtcNow.DateTime);
        await _learningRepository.Update(learning);

        var approver = priceChange.ChangeApprovedBy();

        if (learning.LatestEpisode.FundingPlatform == FundingPlatform.DAS)
        {
            await SendEvent(learning, priceChange, approver);
        }

        _logger.LogInformation("Price change approved by {approver} for learning {learningKey}", approver.ToString(), command.LearningKey);
        return approver;
    }

    public async Task SendEvent(LearningDomainModel learning, PriceHistoryDomainModel priceChange, ApprovedBy approvedBy)
    {
        _logger.LogInformation("Sending ApprenticeshipPriceChangedEvent for learning {learningKey}", learning.Key);
        
        var eventMessage = new LearningPriceChangedEvent()
        {
            LearningKey = learning.Key,
            LearningId = learning.ApprovalsApprenticeshipId,
            EffectiveFromDate = priceChange.EffectiveFromDate,
            ApprovedDate = priceChange.ApprovalDate(),
            ApprovedBy = approvedBy,
            Episode = learning.BuildEpisodeForIntegrationEvent()
        };

        await _messageSession.Publish(eventMessage);
    }
}
