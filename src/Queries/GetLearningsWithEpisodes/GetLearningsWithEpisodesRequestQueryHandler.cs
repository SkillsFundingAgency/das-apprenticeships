using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Extensions;
using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetLearningsWithEpisodes;

public class GetLearningsWithEpisodesRequestQueryHandler : IQueryHandler<GetLearningsWithEpisodesRequest, GetLearningsWithEpisodesResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;
    private readonly ILogger<GetLearningsWithEpisodesRequestQueryHandler> _logger;

    public GetLearningsWithEpisodesRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository, ILogger<GetLearningsWithEpisodesRequestQueryHandler> logger)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
        _logger = logger;
    }

    public async Task<GetLearningsWithEpisodesResponse?> Handle(GetLearningsWithEpisodesRequest query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetLearningsWithEpisodesRequest for Ukprn: {ukprn} CollectionYear: {collectionYear} CollectionPeriod: {collectionPeriod}", query.Ukprn, query.CollectionYear, query.CollectionPeriod);

        var apprenticeships = await _apprenticeshipQueryRepository.GetApprenticeshipsWithEpisodes(query.Ukprn, query.CollectionYear.GetLastDay(query.CollectionPeriod));

        if (apprenticeships == null)
        {
            _logger.LogInformation("No apprenticeships found for {ukprn}", query.Ukprn);
            return null;
        }

        _logger.LogInformation("{numberFound} apprenticeships found for {ukprn}", apprenticeships.Count, query.Ukprn);
        return new GetLearningsWithEpisodesResponse(query.Ukprn, apprenticeships);
    }
}