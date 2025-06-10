using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Extensions;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Infrastructure.Services;
using SFA.DAS.Learning.Types;

namespace SFA.DAS.Learning.Queries.GetLearnerStatus;


public class GetLearnerStatusQueryHandler : IQueryHandler<GetLearnerStatusRequest, GetLearnerStatusResponse?>
{
    private readonly ILearningQueryRepository _learningQueryRepository;
    private readonly ISystemClockService _systemClockService;
    private readonly ILogger<GetLearnerStatusQueryHandler> _logger;

    public GetLearnerStatusQueryHandler(ILearningQueryRepository learningQueryRepository, ISystemClockService systemClockService, ILogger<GetLearnerStatusQueryHandler> logger)
    {
        _learningQueryRepository = learningQueryRepository;
        _systemClockService = systemClockService;
        _logger = logger;
    }

    public async Task<GetLearnerStatusResponse?> Handle(GetLearnerStatusRequest query, CancellationToken cancellationToken = default)
    {
        var startDate = await _learningQueryRepository.GetStartDate(query.ApprenticeshipKey);

        if(startDate == null)
        {
            _logger.LogError("No start date found for apprenticeship key {apprenticeshipKey}", query.ApprenticeshipKey);
            return null;
        }

        var domainLearnerStatus = await _learningQueryRepository.GetLearnerStatus(query.ApprenticeshipKey);

        if (domainLearnerStatus?.LearnerStatus == Learning.Domain.Apprenticeship.LearnerStatus.Withdrawn)
        {
            return new GetLearnerStatusResponse
            {
                LearnerStatus = LearnerStatus.Withdrawn,
                WithdrawalChangedDate = domainLearnerStatus.WithdrawalChangedDate,
                WithdrawalReason = domainLearnerStatus.WithdrawalReason,
                LastCensusDateOfLearning = domainLearnerStatus.LastDayOfLearning?.GetLastCensusDateBefore(),
                LastDayOfLearning = domainLearnerStatus.LastDayOfLearning
            };
        }

        if(startDate.ActualStartDate > _systemClockService.UtcNow.DateTime)
        {
            return new GetLearnerStatusResponse { LearnerStatus = LearnerStatus.WaitingToStart };
        }

        return new GetLearnerStatusResponse { LearnerStatus = LearnerStatus.InLearning }; 
    }
}