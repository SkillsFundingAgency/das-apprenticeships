using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Command.CreatePriceChange
{
    public class CreatePriceChangeCommandHandler : ICommandHandler<CreatePriceChangeCommand, ChangeRequestStatus>
    {
        private readonly IApprenticeshipRepository _apprenticeshipRepository;

        public CreatePriceChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository)
        {
            _apprenticeshipRepository = apprenticeshipRepository;
        }

        public async Task<ChangeRequestStatus> Handle(CreatePriceChangeCommand command,
            CancellationToken cancellationToken = default)
        {
            var returnStatus = ChangeRequestStatus.Created;
            var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);

            if (!Enum.TryParse(command.Initiator, out ChangeInitiator initiator))
                throw new ArgumentException("CreateApprenticeshipPriceChangeRequest should have a valid initiator value set (Provider or Employer)", nameof(command));
            

            if (initiator == ChangeInitiator.Provider)
            {
                apprenticeship.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, DateTime.Now, ChangeRequestStatus.Created, command.UserId, command.Reason, null, DateTime.Now, null, initiator);
            }
            else
            {
                apprenticeship.AddPriceHistory(command.TrainingPrice, command.AssessmentPrice, command.TotalPrice, command.EffectiveFromDate, DateTime.Now, ChangeRequestStatus.Created, null, command.Reason, command.UserId, null, DateTime.Now, initiator);
            }

            await _apprenticeshipRepository.Update(apprenticeship);

            if (initiator == ChangeInitiator.Provider && EmployerApprovalNotRequired(apprenticeship, command))
            {
	            apprenticeship.ProviderSelfApprovePriceChange();
	            returnStatus = ChangeRequestStatus.Approved;
	            await _apprenticeshipRepository.Update(apprenticeship);
			}

            return returnStatus;
        }

        private static bool EmployerApprovalNotRequired(ApprenticeshipDomainModel apprenticeshipDomainModel, CreatePriceChangeCommand command)
        {
            var apprenticeship = apprenticeshipDomainModel.GetEntity();

            if (apprenticeship.TotalPrice >= command.TotalPrice)
            {
                return true;
            }

            return false;
        }
    }
}
