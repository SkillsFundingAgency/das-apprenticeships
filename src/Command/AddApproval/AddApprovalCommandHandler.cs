using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

namespace SFA.DAS.Apprenticeships.Command.AddApproval
{
    public class AddApprovalCommandHandler : ICommandHandler<AddApprovalCommand>
    {
        private readonly IApprenticeshipFactory _apprenticeshipFactory;
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly IApprenticeshipsOuterApiClient _approvalsOuterApiClient;

        public AddApprovalCommandHandler(IApprenticeshipFactory apprenticeshipFactory, IApprenticeshipRepository apprenticeshipRepository, IApprenticeshipsOuterApiClient approvalsOuterApiClient)
        {
            _apprenticeshipFactory = apprenticeshipFactory;
            _apprenticeshipRepository = apprenticeshipRepository;
            _approvalsOuterApiClient = approvalsOuterApiClient;
        }

        public async Task Handle(AddApprovalCommand command, CancellationToken cancellationToken = default)
        {
            var apprenticeship = _apprenticeshipFactory.CreateNew(command.Uln, command.TrainingCode);
            var fundingBandMaximum = await _approvalsOuterApiClient.GetFundingBandMaximum(int.Parse(command.TrainingCode));
            apprenticeship.AddApproval(command.ApprovalsApprenticeshipId, command.UKPRN, command.EmployerAccountId, command.LegalEntityName, command.ActualStartDate, command.PlannedEndDate, command.AgreedPrice, command.FundingEmployerAccountId, command.FundingType, fundingBandMaximum);
            await _apprenticeshipRepository.Add(apprenticeship);
        }
    }
}
