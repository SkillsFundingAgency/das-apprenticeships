using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Command.ApprovePriceChange
{
    public class ApprovePriceChangeCommandHandler : ICommandHandler<ApprovePriceChangeCommand, ChangeApprover>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public ApprovePriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task<ChangeApprover> Handle(ApprovePriceChangeCommand command,
            CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            var approver = apprenticeship.ApprovePriceChange(command.UserId, command.TrainingPrice, command.AssessmentPrice);
            await _apprenticeshipRepository.Update(apprenticeship);
            return approver;
        }
    }
}
