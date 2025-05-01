using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Extensions;
using SFA.DAS.Apprenticeships.Domain.Factories;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.Types;
using FundingPlatform = SFA.DAS.Apprenticeships.Enums.FundingPlatform;

namespace SFA.DAS.Apprenticeships.Command.AddApprenticeship;

public class AddApprenticeshipCommandHandler : ICommandHandler<AddApprenticeshipCommand>
{
    private readonly IApprenticeshipFactory _apprenticeshipFactory;
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IFundingBandMaximumService _fundingBandMaximumService;
    private readonly IMessageSession _messageSession;
    private readonly ILogger<AddApprenticeshipCommandHandler> _logger;

    public AddApprenticeshipCommandHandler(
        IApprenticeshipFactory apprenticeshipFactory,
        IApprenticeshipRepository apprenticeshipRepository,
        IFundingBandMaximumService fundingBandMaximumService,
        IMessageSession messageSession,
        ILogger<AddApprenticeshipCommandHandler> logger)
    {
        _apprenticeshipFactory = apprenticeshipFactory;
        _apprenticeshipRepository = apprenticeshipRepository;
        _fundingBandMaximumService = fundingBandMaximumService;
        _messageSession = messageSession;
        _logger = logger;
    }

    public async Task Handle(AddApprenticeshipCommand command, CancellationToken cancellationToken = default)
    {
        var existingApprenticeship = await _apprenticeshipRepository.Get(command.Uln, command.ApprovalsApprenticeshipId);
        if (existingApprenticeship != null)
        {
            _logger.LogInformation($"Apprenticeship not created as a record already exists with ULN: {command.Uln} and ApprovalsApprenticeshipId: {command.ApprovalsApprenticeshipId}.");
            return;
        }

        _logger.LogInformation("Handling AddApprenticeshipCommand for Approvals Apprenticeship Id: {approvalsApprenticeshipId}", command.ApprovalsApprenticeshipId);

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

        if (apprenticeship.LatestEpisode.FundingPlatform == FundingPlatform.DAS)
        {
            await SendEvent(apprenticeship);
        }
    }

    private async Task SendEvent(ApprenticeshipDomainModel apprenticeship)
    {
        _logger.LogInformation("Sending ApprenticeshipCreatedEvent for Approvals Apprenticeship Id: {approvalsApprenticeshipId}", apprenticeship.ApprovalsApprenticeshipId);
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