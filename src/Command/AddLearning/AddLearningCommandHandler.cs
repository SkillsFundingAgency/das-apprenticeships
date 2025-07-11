using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Apprenticeship;
using SFA.DAS.Learning.Domain.Extensions;
using SFA.DAS.Learning.Domain.Factories;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Infrastructure.Services;
using SFA.DAS.Apprenticeships.Types;
using FundingPlatform = SFA.DAS.Learning.Enums.FundingPlatform;

namespace SFA.DAS.Learning.Command.AddLearning;

public class AddLearningCommandHandler : ICommandHandler<AddLearningCommand>
{
    private readonly ILearningFactory _learningFactory;
    private readonly ILearningRepository _learningRepository;
    private readonly IFundingBandMaximumService _fundingBandMaximumService;
    private readonly IMessageSession _messageSession;
    private readonly ILogger<AddLearningCommandHandler> _logger;

    public AddLearningCommandHandler(
        ILearningFactory learningFactory,
        ILearningRepository learningRepository,
        IFundingBandMaximumService fundingBandMaximumService,
        IMessageSession messageSession,
        ILogger<AddLearningCommandHandler> logger)
    {
        _learningFactory = learningFactory;
        _learningRepository = learningRepository;
        _fundingBandMaximumService = fundingBandMaximumService;
        _messageSession = messageSession;
        _logger = logger;
    }

    public async Task Handle(AddLearningCommand command, CancellationToken cancellationToken = default)
    {
        var existingLearning = await _learningRepository.Get(command.Uln, command.ApprovalsApprenticeshipId);
        if (existingLearning != null)
        {
            _logger.LogInformation($"Learning not created as a record already exists with ULN: {command.Uln} and ApprovalsApprenticeshipId: {command.ApprovalsApprenticeshipId}.");
            return;
        }

        _logger.LogInformation("Handling AddLearningCommand for Approvals Learning Id: {approvalsApprenticeshipId}", command.ApprovalsApprenticeshipId);

        if (command.ActualStartDate == null)
        {
            throw new Exception(
                $"{nameof(command.ActualStartDate)} for Learning ({command.ApprenticeshipHashedId} (Approvals Learning Id: {command.ApprovalsApprenticeshipId}) is null. " +
                $"Learnings funded by DAS should always have an actual start date. ");
        }
        var startDate = command.ActualStartDate.Value;
        var fundingBandMaximum = await _fundingBandMaximumService.GetFundingBandMaximum(int.Parse(command.TrainingCode), startDate);

        if (fundingBandMaximum == null)
            throw new Exception(
                $"No funding band maximum found for course {command.TrainingCode} for given date {startDate:u}. Approvals Learning Id: {command.ApprovalsApprenticeshipId}");

        var learning = _learningFactory.CreateNew(
            command.ApprovalsApprenticeshipId,
            command.Uln,
            command.DateOfBirth,
            command.FirstName,
            command.LastName,
            command.ApprenticeshipHashedId);

        learning.AddEpisode(
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

        await _learningRepository.Add(learning);

        if (learning.LatestEpisode.FundingPlatform == FundingPlatform.DAS)
        {
            await SendEvent(learning);
        }
    }

    private async Task SendEvent(ApprenticeshipDomainModel apprenticeship)
    {
        _logger.LogInformation("Sending ApprenticeshipCreatedEvent for Approvals Learning Id: {approvalsApprenticeshipId}", apprenticeship.ApprovalsApprenticeshipId);
        var apprenticeshipCreatedEvent = new ApprenticeshipCreatedEvent
        {
            ApprenticeshipKey = apprenticeship.Key,
            Uln = apprenticeship.Uln,
            ApprovalsApprenticeshipId = apprenticeship.ApprovalsApprenticeshipId,
            DateOfBirth = apprenticeship.DateOfBirth,
            FirstName = apprenticeship.FirstName,
            LastName = apprenticeship.LastName,
            Episode = apprenticeship.BuildEpisodeForIntegrationEvent()
        };

        await _messageSession.Publish(apprenticeshipCreatedEvent);
    }
}