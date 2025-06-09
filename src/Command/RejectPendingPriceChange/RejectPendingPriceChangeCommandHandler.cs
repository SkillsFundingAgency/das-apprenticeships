using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Command.RejectPendingPriceChange
{
    public class RejectPendingPriceChangeCommandHandler : ICommandHandler<RejectPendingPriceChangeRequest>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public RejectPendingPriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task Handle(RejectPendingPriceChangeRequest command, CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            apprenticeship.RejectPendingPriceChange(command.Reason);
            await _apprenticeshipRepository.Update(apprenticeship);
        }
    }
}
