using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetCurrentPartyIds;

public class GetCurrentPartyIdsRequestQueryHandler : IQueryHandler<GetCurrentPartyIdsRequest, GetCurrentPartyIdsResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;
    private readonly ILogger<GetCurrentPartyIdsRequestQueryHandler> _logger;

    public GetCurrentPartyIdsRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository, ILogger<GetCurrentPartyIdsRequestQueryHandler> logger)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
        _logger = logger;
    }

    public async Task<GetCurrentPartyIdsResponse?> Handle(GetCurrentPartyIdsRequest query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetCurrentPartyIdsRequest for ApprenticeshipKey: {key}", query.ApprenticeshipKey);

        var currentPartyIds = await _apprenticeshipQueryRepository.GetCurrentPartyIds(query.ApprenticeshipKey);

        if (currentPartyIds == null)
        {
            _logger.LogInformation("Apprenticeship with current episode not found for ApprenticeshipKey: {key}", query.ApprenticeshipKey);
            return null;
        }

        return new GetCurrentPartyIdsResponse(currentPartyIds.Ukprn, currentPartyIds.EmployerAccountId, currentPartyIds.ApprovalsApprenticeshipId);
    }
}