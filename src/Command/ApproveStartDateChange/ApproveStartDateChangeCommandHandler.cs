using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Extensions;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Command.ApproveStartDateChange;

public class ApproveStartDateChangeCommandHandler : ICommandHandler<ApproveStartDateChangeCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IMessageSession _messageSession;
    private readonly ILogger<ApproveStartDateChangeCommandHandler> _logger;

    public ApproveStartDateChangeCommandHandler(
        IApprenticeshipRepository apprenticeshipRepository, 
        IMessageSession messageSession,
        ILogger<ApproveStartDateChangeCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _messageSession = messageSession;
        _logger = logger;
    }

    public async Task Handle(ApproveStartDateChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Approving start date change for apprenticeship {apprenticeshipKey}", command.ApprenticeshipKey);

        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);

        var startDateChange = apprenticeship.ApproveStartDateChange(command.UserId);
        await _apprenticeshipRepository.Update(apprenticeship);

        await SendEvent(apprenticeship, startDateChange);

        _logger.LogInformation("Start date change approved for apprenticeship {apprenticeshipKey}", command.ApprenticeshipKey);
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