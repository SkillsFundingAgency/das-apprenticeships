using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Command.CancelPendingPriceChange
{
    public class CancelPendingPriceChangeCommandHandler : ICommandHandler<CancelPendingPriceChangeRequest>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly ILogger<CancelPendingPriceChangeCommandHandler> _logger;

        public CancelPendingPriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository, ILogger<CancelPendingPriceChangeCommandHandler> logger)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
            _logger = logger;
        }

        public async Task Handle(CancelPendingPriceChangeRequest command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Beginning cancel of PriceChangeRequest for ApprenticeshipKey {command.ApprenticeshipKey}");

            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            if(apprenticeship == null)
            {
                _logger.LogError("Apprenticeship not found for key {ApprenticeshipKey}", command.ApprenticeshipKey);
                return;
            }
                
            apprenticeship.CancelPendingPriceChange();
            await _apprenticeshipRepository.Update(apprenticeship);

        }
    }
}
