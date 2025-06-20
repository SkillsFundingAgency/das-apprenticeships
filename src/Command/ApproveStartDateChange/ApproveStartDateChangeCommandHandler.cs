using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Extensions;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;
using SFA.DAS.Apprenticeship.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;

namespace SFA.DAS.Learning.Command.ApproveStartDateChange;

public class ApproveStartDateChangeCommandHandler : ICommandHandler<ApproveStartDateChangeCommand>
{
    private readonly ILearningRepository _learningRepository;
    private readonly IMessageSession _messageSession;
    private readonly ILogger<ApproveStartDateChangeCommandHandler> _logger;

    public ApproveStartDateChangeCommandHandler(
        ILearningRepository learningRepository, 
        IMessageSession messageSession,
        ILogger<ApproveStartDateChangeCommandHandler> logger)
    {
        _learningRepository = learningRepository;
        _messageSession = messageSession;
        _logger = logger;
    }

    public async Task Handle(ApproveStartDateChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Approving start date change for apprenticeship {apprenticeshipKey}", command.LearningKey);

        var apprenticeship = await _learningRepository.Get(command.LearningKey);

        var startDateChange = apprenticeship.ApproveStartDateChange(command.UserId);
        await _learningRepository.Update(apprenticeship);

        if (apprenticeship.LatestEpisode.FundingPlatform == FundingPlatform.DAS)
        {
            await SendEvent(apprenticeship, startDateChange);
        }

        _logger.LogInformation("Start date change approved for apprenticeship {apprenticeshipKey}", command.LearningKey);
    }

    private async Task SendEvent(ApprenticeshipDomainModel apprenticeship, StartDateChangeDomainModel startDateChange)
    {
        _logger.LogInformation("Sending ApprenticeshipStartDateChangedEvent for apprenticeship {apprenticeshipKey}", apprenticeship.Key);
        var approver = startDateChange.GetApprover();

        var eventMessage = new ApprenticeshipStartDateChangedEvent()
        {
            ApprenticeshipKey = apprenticeship.Key,
            ApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
            StartDate = apprenticeship.StartDate,
            ApprovedDate = approver == ApprovedBy.Employer ? startDateChange.EmployerApprovedDate!.Value : startDateChange.ProviderApprovedDate!.Value,
            ProviderApprovedBy = startDateChange.ProviderApprovedBy,
            EmployerApprovedBy = startDateChange.EmployerApprovedBy,
            Initiator = startDateChange.Initiator.ToString()!,
            Episode = apprenticeship.BuildEpisodeForIntegrationEvent()
        };

        await _messageSession.Publish(eventMessage);
    }
}