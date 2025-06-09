using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;

namespace SFA.DAS.Learning.Command.SetPaymentsFrozen;

public class SetPaymentsFrozenCommandHandler : ICommandHandler<SetPaymentsFrozenCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IMessageSession _messageSession;
    private readonly ILogger<SetPaymentsFrozenCommandHandler> _logger;

    public SetPaymentsFrozenCommandHandler(
        IApprenticeshipRepository apprenticeshipRepository,
        IMessageSession messageSession,
        ILogger<SetPaymentsFrozenCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _messageSession = messageSession;
        _logger = logger;
    }

    public async Task Handle(SetPaymentsFrozenCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling SetPaymentsFrozenCommand for apprenticeship {apprenticeshipKey}", command.ApprenticeshipKey);
        
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
        apprenticeship.SetPaymentsFrozen(command.NewPaymentsFrozenStatus, command.UserId, DateTime.Now, command.Reason);
        await _apprenticeshipRepository.Update(apprenticeship);

        if (apprenticeship.LatestEpisode.FundingPlatform == FundingPlatform.DAS)
        {
            if (command.NewPaymentsFrozenStatus)
            {
                _logger.LogInformation("Publishing PaymentsFrozenEvent for apprenticeship {apprenticeshipKey}", command.ApprenticeshipKey);
                await _messageSession.Publish(new PaymentsFrozenEvent { ApprenticeshipKey = command.ApprenticeshipKey });
            }
            else
            {
                _logger.LogInformation("Publishing PaymentsUnfrozenEvent for apprenticeship {apprenticeshipKey}", command.ApprenticeshipKey);
                await _messageSession.Publish(new PaymentsUnfrozenEvent { ApprenticeshipKey = command.ApprenticeshipKey });
            }
        }
    }
}
