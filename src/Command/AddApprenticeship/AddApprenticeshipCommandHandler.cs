using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.Services;

namespace SFA.DAS.Apprenticeships.Command.AddApprenticeship
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
            if (command.ActualStartDate == null)
            {
                throw new Exception(
                    $"{nameof(command.ActualStartDate)} for Apprenticeship ({command.ApprenticeshipHashedId} (Approvals Apprenticeship Id: {command.ApprovalsApprenticeshipId}) is null. " +
                    $"Apprenticeships funded by DAS should always have an actual start date. ");
            }
            var startDate = command.ActualStartDate.Value;
            var fundingBandMaximum = await _fundingBandMaximumService.GetFundingBandMaximum(int.Parse(command.TrainingCode), startDate);

            if (fundingBandMaximum == null)
                throw new Exception(
                    $"No funding band maximum found for course {command.TrainingCode} for given date {startDate:u}. Approvals Apprenticeship Id: {command.ApprovalsApprenticeshipId}");
            
            var apprenticeship = _apprenticeshipFactory.CreateNew(
                command.ApprovalsApprenticeshipId,
                command.Uln,
                command.DateOfBirth,
                command.FirstName,
                command.LastName,
                command.ApprenticeshipHashedId);

            apprenticeship.AddEpisode(
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
                command.TrainingCourseVersion);

            await _apprenticeshipRepository.Add(apprenticeship);
        }
    }
}