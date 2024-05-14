using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.RejectStartDateChange;

public class RejectStartDateChangeCommandHandler : ICommandHandler<RejectStartDateChangeCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;

    public RejectStartDateChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
    }

    public async Task Handle(RejectStartDateChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
        apprenticeship.RejectStartDateChange(command.Reason);
        await _apprenticeshipRepository.Update(apprenticeship);
    }
}