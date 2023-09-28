using NServiceBus;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Command.ChangePrice
{
    public class ChangePriceCommandHandler : ICommandHandler<ChangePriceCommand>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly IFundingBandMaximumService _fundingBandMaximumService;
        private readonly IMessageSession _messageSession;

        public ChangePriceCommandHandler(
            IApprenticeshipRepository apprenticeshipRepository,
            IFundingBandMaximumService fundingBandMaximumService,
            IMessageSession messageSession)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
            _fundingBandMaximumService = fundingBandMaximumService;
            _messageSession = messageSession;
        }

        public async Task Handle(ChangePriceCommand command, CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _apprenticeshipRepository.GetByApprenticeshipId(command.ApprovalsApprenticeshipId)
                                 ?? throw new ArgumentException($"No apprenticeship with ApprovalsApprenticeshipId: {command.ApprovalsApprenticeshipId} found");

            await UpdateData(command, apprenticeship);
            await PublishEvent(command, apprenticeship);
        }

        private async Task UpdateData(ChangePriceCommand command, Apprenticeship apprenticeship)
        {
            apprenticeship.AddPriceChange(
                command.ApprovedDate,
                command.AssessmentPrice,
                command.TrainingPrice,
                command.EffectiveFrom);

            await _apprenticeshipRepository.Update(apprenticeship);
        }

        private async Task PublishEvent(ChangePriceCommand command, Apprenticeship apprenticeship)
        {
            var @event = new ApprenticeshipPriceChangedEvent
            {
                ApprenticeshipKey = apprenticeship.Key,
                AssessmentPrice = command.AssessmentPrice,
                TrainingPrice = command.TrainingPrice,
                TotalPrice = command.AssessmentPrice + command.TrainingPrice,
                ApprovedDate = command.ApprovedDate,
                EffectiveFrom = command.EffectiveFrom
            };

            await _messageSession.Publish(@event);
        }
    }
}
