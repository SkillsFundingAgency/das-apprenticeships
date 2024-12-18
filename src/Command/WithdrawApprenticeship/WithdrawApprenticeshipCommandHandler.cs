using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;
using SFA.DAS.Apprenticeships.Infrastructure.Services;

namespace SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;

public class WithdrawApprenticeshipCommandHandler : ICommandHandler<WithdrawApprenticeshipCommand, WithdrawApprenticeshipResponse>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IApprenticeshipsOuterApiClient _apprenticeshipsOuterApiClient;
    private readonly ISystemClockService _systemClockService;
    private readonly IValidator<WithdrawApprenticeshipCommand> _validator;
    private ILogger<WithdrawApprenticeshipCommandHandler> _logger;

    public WithdrawApprenticeshipCommandHandler(
        IApprenticeshipRepository apprenticeshipRepository, 
        IApprenticeshipsOuterApiClient apprenticeshipsOuterApiClient,
        ISystemClockService systemClockService,
        IValidator<WithdrawApprenticeshipCommand> validator,
        ILogger<WithdrawApprenticeshipCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _apprenticeshipsOuterApiClient = apprenticeshipsOuterApiClient;
        _systemClockService = systemClockService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<WithdrawApprenticeshipResponse> Handle(WithdrawApprenticeshipCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling WithdrawApprenticeshipCommand for ULN {command.ULN}");
        var apprenticeship = await _apprenticeshipRepository.GetByUln(command.ULN);

        var academicYear = await _apprenticeshipsOuterApiClient.GetAcademicYear(_systemClockService.UtcNow.DateTime);

        if (!_validator.IsValid(command, out var message, apprenticeship, academicYear.EndDate))
        {
            return new WithdrawApprenticeshipResponse { IsSuccess = false, Message = message };
        }

        _logger.LogInformation($"Validation passed, Withdrawing apprenticeship for ULN {command.ULN}");
        apprenticeship!.WithdrawApprenticeship(command.ProviderApprovedBy, command.LastDayOfLearning, GetReason(command), _systemClockService.UtcNow.DateTime);
        await _apprenticeshipRepository.Update(apprenticeship);

        _logger.LogInformation($"Apprenticeship withdrawn for ULN {command.ULN}");
        return new WithdrawApprenticeshipResponse { IsSuccess = true };
    }

    private string GetReason(WithdrawApprenticeshipCommand command)
    {
        if(command.Reason == WithdrawReason.Other.ToString())
        {
            return command.ReasonText;
        }

        return command.Reason;
    }
}
