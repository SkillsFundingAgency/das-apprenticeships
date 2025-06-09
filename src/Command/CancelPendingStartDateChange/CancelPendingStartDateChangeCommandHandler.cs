using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Command.CancelPendingStartDateChange;

public class CancelPendingStartDateChangeCommandHandler : ICommandHandler<CancelPendingStartDateChangeRequest>
{
	private readonly ILearningRepository _learningRepository;

	public CancelPendingStartDateChangeCommandHandler(ILearningRepository learningRepository)
	{
		_learningRepository = learningRepository;
	}

	public async Task Handle(CancelPendingStartDateChangeRequest command, CancellationToken cancellationToken = default)
	{
		var apprenticeship = await _learningRepository.Get(command.ApprenticeshipKey);
		apprenticeship.CancelPendingStartDateChange();
		await _learningRepository.Update(apprenticeship);
	}
}