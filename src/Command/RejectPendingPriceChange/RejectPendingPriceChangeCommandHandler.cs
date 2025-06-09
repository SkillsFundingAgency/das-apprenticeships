using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Command.RejectPendingPriceChange
{
    public class RejectPendingPriceChangeCommandHandler : ICommandHandler<RejectPendingPriceChangeRequest>
    {
        private readonly ILearningRepository _learningRepository;

        public RejectPendingPriceChangeCommandHandler(ILearningRepository learningRepository)
        {
            _learningRepository = learningRepository;
        }

        public async Task Handle(RejectPendingPriceChangeRequest command, CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _learningRepository.Get(command.ApprenticeshipKey);
            apprenticeship.RejectPendingPriceChange(command.Reason);
            await _learningRepository.Update(apprenticeship);
        }
    }
}
