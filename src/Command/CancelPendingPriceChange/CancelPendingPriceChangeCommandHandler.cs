using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Command.CancelPendingPriceChange
{
    public class CancelPendingPriceChangeCommandHandler : ICommandHandler<CancelPendingPriceChangeRequest>
    {
        private readonly ILearningRepository _learningRepository;

        public CancelPendingPriceChangeCommandHandler(ILearningRepository learningRepository)
        {
            _learningRepository = learningRepository;
        }

        public async Task Handle(CancelPendingPriceChangeRequest command, CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _learningRepository.Get(command.ApprenticeshipKey);
            apprenticeship.CancelPendingPriceChange();
            await _learningRepository.Update(apprenticeship);
        }
    }
}
