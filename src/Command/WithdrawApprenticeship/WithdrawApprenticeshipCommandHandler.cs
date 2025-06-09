using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Domain.Validators;
using SFA.DAS.Learning.Infrastructure.ApprenticeshipsOuterApiClient;
using SFA.DAS.Learning.Infrastructure.Services;
using SFA.DAS.Learning.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;

namespace SFA.DAS.Learning.Command.WithdrawApprenticeship;

public class WithdrawApprenticeshipCommandHandler : ICommandHandler<WithdrawApprenticeshipCommand, Outcome>
{
    private readonly ILearningRepository _learningRepository;
    private readonly IApprenticeshipsOuterApiClient _apprenticeshipsOuterApiClient;
    private readonly ISystemClockService _systemClockService;
    private readonly IValidator<WithdrawDomainRequest> _validator;
    private readonly IMessageSession _messageSession;
    private ILogger<WithdrawApprenticeshipCommandHandler> _logger;

    public WithdrawApprenticeshipCommandHandler(
        ILearningRepository learningRepository, 
        IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient,
        ISystemClockService systemClockService,
        IValidator<WithdrawDomainRequest> validator,
        IMessageSession messageSession,
        ILogger<WithdrawApprenticeshipCommandHandler> logger)
    {
        _learningRepository = learningRepository;
        _apprenticeshipsOuterApiClient = apprenticeshipsOuterApiClient;
        _systemClockService = systemClockService;
        _validator = validator;
        _messageSession = messageSession;
        _logger = logger;
    }

    public async Task<Outcome> Handle(WithdrawApprenticeshipCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling WithdrawApprenticeshipCommand for ULN {command.ULN}");
        var apprenticeship = await _learningRepository.GetByUln(command.ULN);

        if (apprenticeship == null)
        {
            throw new InvalidOperationException($"Unable to find apprenticeship by ULN {command.ULN}");
        }

        var academicYear = await _apprenticeshipsOuterApiClient.GetAcademicYear(_systemClockService.UtcNow.DateTime);

        if (!_validator.IsValid(command.ToDomainRequest(), out var message, apprenticeship, academicYear.EndDate))
        {
            return Outcome.Fail(message);
        }

        _logger.LogInformation($"Validation passed, Withdrawing apprenticeship for ULN {command.ULN}");

        var reason = GetReason(command);
        apprenticeship.WithdrawApprenticeship(command.ProviderApprovedBy, command.LastDayOfLearning, reason, _systemClockService.UtcNow.DateTime);
        await _learningRepository.Update(apprenticeship);

        if (apprenticeship.LatestEpisode.FundingPlatform == FundingPlatform.DAS)
        {
            await SendEvent(apprenticeship, reason, command.LastDayOfLearning);
        }

        _logger.LogInformation($"Sending Notification(s) for withdrawal of apprenticeship for ULN {command.ULN}");
        await _apprenticeshipsOuterApiClient.HandleWithdrawalNotifications(apprenticeship.Key, new HandleWithdrawalNotificationsRequest { LastDayOfLearning = command.LastDayOfLearning, Reason = command.Reason }, command.ServiceBearerToken);

        _logger.LogInformation($"Apprenticeship withdrawn for ULN {command.ULN}");
        return Outcome.Success();
    }

    private string GetReason(WithdrawApprenticeshipCommand command)
    {
        if(command.Reason == WithdrawReason.Other.ToString())
        {
            return command.ReasonText;
        }

        return command.Reason;
    }

    private async Task SendEvent(ApprenticeshipDomainModel apprenticeship, string reason, DateTime lastDayOfLearning)
    {
        _logger.LogInformation("Publishing ApprenticeshipWithdrawnEvent for {apprenticeshipKey}", apprenticeship.Key);
        var message = new ApprenticeshipWithdrawnEvent
        {
            ApprenticeshipKey = apprenticeship.Key,
            ApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
            Reason = reason,
            LastDayOfLearning = lastDayOfLearning,
            EmployerAccountId = apprenticeship.LatestEpisode.EmployerAccountId
        };

        await _messageSession.Publish(message);
    }
}
