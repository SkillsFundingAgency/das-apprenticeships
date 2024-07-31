using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.ApproveStartDateChange;

public class ApproveStartDateChangeCommandHandler : ICommandHandler<ApproveStartDateChangeCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;

    public ApproveStartDateChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
    }

    public async Task Handle(ApproveStartDateChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
        apprenticeship.ApproveStartDateChange(command.UserId);
        await _apprenticeshipRepository.Update(apprenticeship);
    }
}