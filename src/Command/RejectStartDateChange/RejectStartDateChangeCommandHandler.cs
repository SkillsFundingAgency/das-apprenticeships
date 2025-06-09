using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Command.RejectStartDateChange;

public class RejectStartDateChangeCommandHandler : ICommandHandler<RejectStartDateChangeCommand>
{
    private readonly ILearningRepository _learningRepository;

    public RejectStartDateChangeCommandHandler(ILearningRepository learningRepository)
    {
        _learningRepository = learningRepository;
    }

    public async Task Handle(RejectStartDateChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _learningRepository.Get(command.ApprenticeshipKey);
        apprenticeship.RejectStartDateChange(command.Reason);
        await _learningRepository.Update(apprenticeship);
    }
}