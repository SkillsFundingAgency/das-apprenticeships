using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Queries.GetLearnerStatus;


public class GetLearnerStatusQueryHandler : IQueryHandler<GetLearnerStatusRequest, GetLearnerStatusResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;
    private readonly ISystemClockService _systemClockService;
    private readonly ILogger<GetLearnerStatusQueryHandler> _logger;

    public GetLearnerStatusQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository, ISystemClockService systemClockService, ILogger<GetLearnerStatusQueryHandler> logger)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
        _systemClockService = systemClockService;
        _logger = logger;
    }

    public async Task<GetLearnerStatusResponse?> Handle(GetLearnerStatusRequest query, CancellationToken cancellationToken = default)
    {
        var startDate = await _apprenticeshipQueryRepository.GetStartDate(query.ApprenticeshipKey);

        if(startDate == null)
        {
            _logger.LogError($"No start date found for apprenticeship key {query.ApprenticeshipKey}");
            return null;
        }

        if(startDate.ActualStartDate > _systemClockService.UtcNow.DateTime)
        {
            return new GetLearnerStatusResponse { LearnerStatus = LearnerStatus.WaitingToStart };
        }

        return new GetLearnerStatusResponse { LearnerStatus = LearnerStatus.InLearning }; //TODO: Add logic for break in learning, withdrawn and completed
    }
}