using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.Infrastructure.Services;

namespace SFA.DAS.Apprenticeships.Command.AddApproval
{
    public class AddApprenticeshipCommandHandler : ICommandHandler<AddApprenticeshipCommand>
    {
        private readonly IApprenticeshipFactory _apprenticeshipFactory;
        private readonly IApprenticeshipRepository _apprenticeshipRepository;
        private readonly IFundingBandMaximumService _fundingBandMaximumService;

        public AddApprenticeshipCommandHandler(IApprenticeshipFactory apprenticeshipFactory, IApprenticeshipRepository apprenticeshipRepository, IFundingBandMaximumService fundingBandMaximumService)
        {
            _apprenticeshipFactory = apprenticeshipFactory;
            _apprenticeshipRepository = apprenticeshipRepository;
            _fundingBandMaximumService = fundingBandMaximumService;
        }

        public async Task Handle(AddApprenticeshipCommand command, CancellationToken cancellationToken = default)
        {
            var startDate = command.FundingPlatform == FundingPlatform.DAS ? command.ActualStartDate : command.PlannedStartDate;
            var fundingBandMaximum = await _fundingBandMaximumService.GetFundingBandMaximum(int.Parse(command.TrainingCode), startDate);

            if (fundingBandMaximum == null)
                throw new Exception(
                    $"No funding band maximum found for course {command.TrainingCode} for given date {startDate?.ToString("u")}. Approvals Apprenticeship Id: {command.ApprovalsApprenticeshipId}");
            
            var apprenticeship = _apprenticeshipFactory.CreateNew(
                command.Uln,
                command.DateOfBirth,
                command.FirstName,
                command.LastName,
                command.ApprenticeshipHashedId);

            apprenticeship.AddEpisode(
                command.ApprovalsApprenticeshipId,
                command.UKPRN,
                command.EmployerAccountId,
                startDate,
                command.PlannedEndDate,
                command.TotalPrice,
                command.TrainingPrice,
                command.EndPointAssessmentPrice,
                command.FundingType,
                command.FundingPlatform,
                fundingBandMaximum.Value,
                command.FundingEmployerAccountId,
                command.LegalEntityName,
                command.AccountLegalEntityId,
                command.TrainingCode,
                command.TrainingCourseVersion
                //,,(!command.PlannedStartDate.HasValue) || (command.PlannedStartDate.GetValueOrDefault().Year == 1) ? null : command.PlannedStartDate.Value, //todo verify this original logic and whether to incorporate it
                );

            await _apprenticeshipRepository.Add(apprenticeship);
        }
    }
}
