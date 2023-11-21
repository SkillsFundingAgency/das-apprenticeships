using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure.Services;

namespace SFA.DAS.Apprenticeships.Command.AddApproval
{
    public class AddApprovalCommandHandler : ICommandHandler<AddApprovalCommand>
    {
        private readonly IApprenticeshipFactory _apprenticeshipFactory;
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly IFundingBandMaximumService _fundingBandMaximumService;

        public AddApprovalCommandHandler(IApprenticeshipFactory apprenticeshipFactory, IApprenticeshipRepository apprenticeshipRepository, IFundingBandMaximumService fundingBandMaximumService)
        {
            _apprenticeshipFactory = apprenticeshipFactory;
            _apprenticeshipRepository = apprenticeshipRepository;
            _fundingBandMaximumService = fundingBandMaximumService;
        }

        public async Task Handle(AddApprovalCommand command, CancellationToken cancellationToken = default)
        {
            var startDate = command.FundingPlatform == FundingPlatform.DAS ? command.ActualStartDate : command.PlannedStartDate;
            var fundingBandMaximum = await _fundingBandMaximumService.GetFundingBandMaximum(int.Parse(command.TrainingCode), startDate);

            if (fundingBandMaximum == null)
                throw new Exception(
                    $"No funding band maximum found for course {command.TrainingCode} for given date {startDate?.ToString("u")}. Approvals Apprenticeship Id: {command.ApprovalsApprenticeshipId}");
            
            var apprenticeship = _apprenticeshipFactory.CreateNew(
                command.Uln,
                command.TrainingCode,
                command.DateOfBirth,
                command.FirstName,
                command.LastName,
                command.TrainingPrice,
                command.EndPointAssessmentPrice,
                command.AgreedPrice,
                command.ApprenticeshipHashedId,
                fundingBandMaximum.Value,
                command.ActualStartDate,
                command.PlannedEndDate);

            

            apprenticeship.AddApproval(
                command.ApprovalsApprenticeshipId,
                command.UKPRN,
                command.EmployerAccountId,
                command.LegalEntityName,
                command.ActualStartDate,
                command.PlannedEndDate,
                command.AgreedPrice,
                command.FundingEmployerAccountId,
                command.FundingType,
                fundingBandMaximum.Value,
                (!command.PlannedStartDate.HasValue) || (command.PlannedStartDate.GetValueOrDefault().Year == 1) ? null : command.PlannedStartDate.Value,
                command.FundingPlatform);
            await _apprenticeshipRepository.Add(apprenticeship);
        }
    }
}
