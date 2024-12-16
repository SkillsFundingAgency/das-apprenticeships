using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Command.AddApprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;

public class WithdrawApprenticeshipCommandHandler : ICommandHandler<WithdrawApprenticeshipCommand, WithdrawApprenticeshipResponse>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private ILogger<WithdrawApprenticeshipCommandHandler> _logger;

    public WithdrawApprenticeshipCommandHandler(IApprenticeshipRepository apprenticeshipRepository, ILogger<WithdrawApprenticeshipCommandHandler> logger)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _logger = logger;
    }

    public async Task<WithdrawApprenticeshipResponse> Handle(WithdrawApprenticeshipCommand command, CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _apprenticeshipRepository.GetByUln(command.ULN);

        if(!command.IsValidWithdrawal(apprenticeship, out var message))
        {
            return new WithdrawApprenticeshipResponse { IsSuccess = false, Message = message };
        }

        apprenticeship!.WithdrawApprenticeship(command.ProviderApprovedBy, command.LastDayOfLearning, GetReason(command));

        await _apprenticeshipRepository.Update(apprenticeship);
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
