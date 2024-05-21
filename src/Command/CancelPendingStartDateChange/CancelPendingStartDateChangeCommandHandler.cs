using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.CancelPendingStartDateChange;

public class CancelPendingStartDateChangeCommandHandler : ICommandHandler<CancelPendingStartDateChangeRequest>
{
	private readonly IApprenticeshipRepository _apprenticeshipRepository;

	public CancelPendingStartDateChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
	{
		_apprenticeshipRepository = apprenticeshipRepository;
	}

	public async Task Handle(CancelPendingStartDateChangeRequest command, CancellationToken cancellationToken = default)
	{
		var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
		apprenticeship.CancelPendingStartDateChange();
		await _apprenticeshipRepository.Update(apprenticeship);
	}
}