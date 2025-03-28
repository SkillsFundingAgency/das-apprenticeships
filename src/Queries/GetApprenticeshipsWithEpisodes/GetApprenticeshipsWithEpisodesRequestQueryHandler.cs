using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Domain.Extensions;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsWithEpisodes;

public class GetApprenticeshipsWithEpisodesRequestQueryHandler : IQueryHandler<GetApprenticeshipsWithEpisodesRequest, GetApprenticeshipsWithEpisodesResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;
    private readonly ILogger<GetApprenticeshipsWithEpisodesRequestQueryHandler> _logger;

    public GetApprenticeshipsWithEpisodesRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository, ILogger<GetApprenticeshipsWithEpisodesRequestQueryHandler> logger)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
        _logger = logger;
    }

    public async Task<GetApprenticeshipsWithEpisodesResponse?> Handle(GetApprenticeshipsWithEpisodesRequest query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetApprenticeshipsWithEpisodesRequest for Ukprn: {ukprn} CollectionYear: {collectionYear} CollectionPeriod: {collectionPeriod}", query.Ukprn, query.CollectionYear, query.CollectionPeriod);

        var apprenticeships = await _apprenticeshipQueryRepository.GetApprenticeshipsWithEpisodes(query.Ukprn, query.CollectionYear.ToDateTime(query.CollectionPeriod));

        if (apprenticeships == null)
        {
            _logger.LogInformation("No apprenticeships found for {ukprn}", query.Ukprn);
            return null;
        }

        _logger.LogInformation("{numberFound} apprenticeships found for {ukprn}", apprenticeships.Count, query.Ukprn);
        return new GetApprenticeshipsWithEpisodesResponse(query.Ukprn, apprenticeships);
    }
}