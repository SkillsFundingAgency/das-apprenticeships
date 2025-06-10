using Microsoft.Extensions.Logging;
using SFA.DAS.Learning.Domain.Extensions;
using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetLearningsWithEpisodes;

public class GetLearningsWithEpisodesRequestQueryHandler : IQueryHandler<GetLearningsWithEpisodesRequest, GetLearningsWithEpisodesResponse?>
{
    private readonly ILearningQueryRepository _learningQueryRepository;
    private readonly ILogger<GetLearningsWithEpisodesRequestQueryHandler> _logger;

    public GetLearningsWithEpisodesRequestQueryHandler(ILearningQueryRepository learningQueryRepository, ILogger<GetLearningsWithEpisodesRequestQueryHandler> logger)
    {
        _learningQueryRepository = learningQueryRepository;
        _logger = logger;
    }

    public async Task<GetLearningsWithEpisodesResponse?> Handle(GetLearningsWithEpisodesRequest query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetLearningsWithEpisodesRequest for Ukprn: {ukprn} CollectionYear: {collectionYear} CollectionPeriod: {collectionPeriod}", query.Ukprn, query.CollectionYear, query.CollectionPeriod);

        var learnings = await _learningQueryRepository.GetLearningsWithEpisodes(query.Ukprn, query.CollectionYear.GetLastDay(query.CollectionPeriod));

        if (learnings == null)
        {
            _logger.LogInformation("No learnings found for {ukprn}", query.Ukprn);
            return null;
        }

        _logger.LogInformation("{numberFound} apprenticeships found for {ukprn}", learnings.Count, query.Ukprn);
        return new GetLearningsWithEpisodesResponse(query.Ukprn, learnings);
    }
}