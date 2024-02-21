using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Command.AddPriceHistory
{
    public class CreateApprenticeshipPriceChangeCommandHandler : ICommandHandler<CreateApprenticeshipPriceChangeRequest>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public CreateApprenticeshipPriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task Handle(CreateApprenticeshipPriceChangeRequest command,
            CancellationToken cancellationToken = default)
        {
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
            if (string.Equals(command.Requester, PriceChangeRequester.Provider.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                apprenticeship.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, DateTime.Now, PriceChangeRequestStatus.Created, command.UserId, command.Reason, null, DateTime.Now, null);
            }
            else if (string.Equals(command.Requester, PriceChangeRequester.Employer.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                apprenticeship.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, DateTime.Now, PriceChangeRequestStatus.Created, null, command.Reason, command.UserId, null, DateTime.Now);
            }
            else
            {
                throw new ArgumentException("CreateApprenticeshipPriceChangeRequest should have a valid requester value set (Provider or Employer)", nameof(command));
            }

            await _apprenticeshipRepository.Update(apprenticeship);
        }
    }
}
