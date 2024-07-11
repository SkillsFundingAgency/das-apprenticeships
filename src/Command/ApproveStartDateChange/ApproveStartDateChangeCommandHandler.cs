using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.Services;

namespace SFA.DAS.Apprenticeships.Command.ApproveStartDateChange;

public class ApproveStartDateChangeCommandHandler : ICommandHandler<ApproveStartDateChangeCommand>
{
    private readonly IApprenticeshipRepository _apprenticeshipRepository;
    private readonly IFundingBandMaximumService _fundingBandMaximumService;

    public ApproveStartDateChangeCommandHandler(IApprenticeshipRepository apprenticeshipRepository, IFundingBandMaximumService fundingBandMaximumService)
    {
        _apprenticeshipRepository = apprenticeshipRepository;
        _fundingBandMaximumService = fundingBandMaximumService;
    }

    public async Task Handle(ApproveStartDateChangeCommand command,
        CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _apprenticeshipRepository.Get(command.ApprenticeshipKey);
        var trainingCode = apprenticeship.LatestEpisode.TrainingCode;
        var newStartDate = apprenticeship.PendingStartDateChange?.ActualStartDate;
        
        //todo do we need to do this re-fetching of the fundingbandmax when new start date is approved (we weren't previously)?
        //if so then will need to check if this needs considering in Earnings domain too
        var fundingBandMaximum = await _fundingBandMaximumService.GetFundingBandMaximum(int.Parse(trainingCode), newStartDate);
        if (fundingBandMaximum == null)
            throw new Exception(
                $"No funding band maximum found for course {trainingCode} for given date {newStartDate:u}. Approvals Apprenticeship Id: {apprenticeship.ApprovalsApprenticeshipId}");

        apprenticeship.ApproveStartDateChange(command.UserId, fundingBandMaximum.Value);
        await _apprenticeshipRepository.Update(apprenticeship);
    }
}