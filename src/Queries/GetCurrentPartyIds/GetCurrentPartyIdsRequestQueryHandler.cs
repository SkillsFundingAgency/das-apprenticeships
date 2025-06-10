using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetCurrentPartyIds;

public class GetCurrentPartyIdsRequestQueryHandler : IQueryHandler<GetCurrentPartyIdsRequest, GetCurrentPartyIdsResponse?>
{
    private readonly ILearningQueryRepository _learningQueryRepository;
    private readonly ILogger<GetCurrentPartyIdsRequestQueryHandler> _logger;

    public GetCurrentPartyIdsRequestQueryHandler(ILearningQueryRepository learningQueryRepository, ILogger<GetCurrentPartyIdsRequestQueryHandler> logger)
    {
        _learningQueryRepository = learningQueryRepository;
        _logger = logger;
    }

    public async Task<GetCurrentPartyIdsResponse?> Handle(GetCurrentPartyIdsRequest query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetCurrentPartyIdsRequest for ApprenticeshipKey: {key}", query.ApprenticeshipKey);

        var currentPartyIds = await _learningQueryRepository.GetCurrentPartyIds(query.ApprenticeshipKey);

        if (currentPartyIds == null)
        {
            _logger.LogInformation("Learning with current episode not found for ApprenticeshipKey: {key}", query.ApprenticeshipKey);
            return null;
        }

        return new GetCurrentPartyIdsResponse(currentPartyIds.Ukprn, currentPartyIds.EmployerAccountId, currentPartyIds.ApprovalsApprenticeshipId);
    }
}