using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Command.CreateStartDateChange;

public class CreateStartDateChangeCommandHandler : ICommandHandler<CreateStartDateChangeCommand>
{
    private readonly ILearningRepository _learningRepository;

    public CreateStartDateChangeCommandHandler(ILearningRepository learningRepository)
    {
        _learningRepository = learningRepository;
    }

    public async Task Handle(CreateStartDateChangeCommand command, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse(command.Initiator, out ChangeInitiator initiator))
            throw new ArgumentException("CreateApprenticeshipStartDateChangeRequest should have a valid initiator value set (Provider or Employer)", nameof(command));

        var apprenticeship = await _learningRepository.Get(command.ApprenticeshipKey);

        if (initiator == ChangeInitiator.Provider)
        {
            apprenticeship.AddStartDateChange(command.ActualStartDate, command.PlannedEndDate, command.Reason, command.UserId, DateTime.Now, null, null, DateTime.Now, ChangeRequestStatus.Created, ChangeInitiator.Provider);
        }
        else
        {
            apprenticeship.AddStartDateChange(command.ActualStartDate, command.PlannedEndDate, command.Reason, null, null, command.UserId, DateTime.Now, DateTime.Now, ChangeRequestStatus.Created, ChangeInitiator.Employer);
        }

        await _learningRepository.Update(apprenticeship);
    }
}