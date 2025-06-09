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
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IMessageSession _messageSession;
    private readonly ISystemClockService _systemClockService;
    private readonly ILogger<ApprovePriceChangeCommandHandler> _logger;

    public ApprovePriceChangeCommandHandler(
        IApprenticeshipRepository apprenticeshipRepository,
        IMessageSession messageSession,
        ISystemClockService systemClockService,
        ILogger<ApprovePriceChangeCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _messageSession = messageSession;
        _systemClockService = systemClockService;
        _logger = logger;
    }

    public async Task<ApprovedBy> Handle(ApprovePriceChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Approving price change for apprenticeship {apprenticeshipKey}", command.ApprenticeshipKey);

        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
        var priceChange = apprenticeship.ApprovePriceChange(command.UserId, command.TrainingPrice, command.AssessmentPrice, _systemClockService.UtcNow.DateTime);
        await _apprenticeshipRepository.Update(apprenticeship);

        var approver = priceChange.ChangeApprovedBy();

        if (apprenticeship.LatestEpisode.FundingPlatform == FundingPlatform.DAS)
        {
            await SendEvent(apprenticeship, priceChange, approver);
        }

        _logger.LogInformation("Price change approved by {approver} for apprenticeship {apprenticeshipKey}", approver.ToString(), command.ApprenticeshipKey);
        return approver;
    }

    public async Task SendEvent(ApprenticeshipDomainModel apprenticeship, PriceHistoryDomainModel priceChange, ApprovedBy approvedBy)
    {
        _logger.LogInformation("Sending ApprenticeshipPriceChangedEvent for apprenticeship {apprenticeshipKey}", apprenticeship.Key);
        
        var eventMessage = new ApprenticeshipPriceChangedEvent()
        {
            ApprenticeshipKey = apprenticeship.Key,
            ApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
            EffectiveFromDate = priceChange.EffectiveFromDate,
            ApprovedDate = priceChange.ApprovalDate(),
            ApprovedBy = approvedBy,
            Episode = apprenticeship.BuildEpisodeForIntegrationEvent()
        };

        await _messageSession.Publish(eventMessage);
    }
}
