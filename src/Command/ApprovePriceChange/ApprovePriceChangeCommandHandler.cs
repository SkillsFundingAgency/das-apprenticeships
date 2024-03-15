using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.ApprovePriceChange
{
    public class ApprovePriceChangeCommandHandler : ICommandHandler<ApprovePriceChangeCommand>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public ApprovePriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task Handle(ApprovePriceChangeCommand command,
            CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);

            if(command.IsProviderApproving)
            {
                apprenticeship.ApprovePriceChange(command.UserId, command.TrainingPrice!.Value, command.AssessmentPrice!.Value);
            }
            else
            {
                apprenticeship.ApprovePriceChange(command.UserId);
            }

            await _apprenticeshipRepository.Update(apprenticeship);
        }
    }
}
