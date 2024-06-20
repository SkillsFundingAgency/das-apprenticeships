using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.SetPaymentsFrozen;

public class SetPaymentsFrozenCommandHandler : ICommandHandler<SetPaymentsFrozenCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;

    public SetPaymentsFrozenCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
    }

    public async Task Handle(SetPaymentsFrozenCommand command, CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
        apprenticeship.SetPaymentsFrozen(command.NewPaymentsFrozenStatus, command.UserId, DateTime.Now, command.Reason);
        await _apprenticeshipRepository.Update(apprenticeship);
    }
}
