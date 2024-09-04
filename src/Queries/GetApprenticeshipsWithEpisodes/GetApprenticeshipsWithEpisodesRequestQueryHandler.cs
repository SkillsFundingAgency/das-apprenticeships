using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipsWithEpisodes;

public class GetApprenticeshipsWithEpisodesRequestQueryHandler : IQueryHandler<GetApprenticeshipsWithEpisodesRequest, GetApprenticeshipsWithEpisodesResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipsWithEpisodesRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipsWithEpisodesResponse?> Handle(GetApprenticeshipsWithEpisodesRequest query, CancellationToken cancellationToken = default)
    {
        var apprenticeships = await _apprenticeshipQueryRepository.GetApprenticeshipsWithEpisodes(query.Ukprn);

        if (apprenticeships == null) return null;

        return new GetApprenticeshipsWithEpisodesResponse(query.Ukprn, apprenticeships);
    }
}