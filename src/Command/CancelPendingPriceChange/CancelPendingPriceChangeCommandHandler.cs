using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.CancelPendingPriceChange
{
    public class CancelPendingPriceChangeCommandHandler : ICommandHandler<CancelPendingPriceChangeRequest>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public CancelPendingPriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task Handle(CancelPendingPriceChangeRequest command, CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            apprenticeship.CancelPendingPriceChange();
            await _apprenticeshipRepository.Update(apprenticeship);
        }
    }
}
