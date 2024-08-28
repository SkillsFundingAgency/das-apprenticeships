using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeship;

public class GetApprenticeshipsRequest : IQuery
{
    public long Ukprn { get; set; }
}

public class GetApprenticeshipsRequestQueryHandler : IQueryHandler<GetApprenticeshipsRequest, GetApprenticeshipsResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipsRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipsResponse?> Handle(GetApprenticeshipsRequest query, CancellationToken cancellationToken = default)
    {
        var apprenticeships = await _apprenticeshipQueryRepository.GetApprenticeshipsWithEpisodes(query.Ukprn);

        if (apprenticeships == null) return null;

        return new GetApprenticeshipsResponse(query.Ukprn, apprenticeships);
    }
}