using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeship;

public class GetApprenticeshipRequest : IQuery
{
    public long Ukprn { get; set; }
    public string Uln { get; set; } = null!;
}

public class GetApprenticeshipsRequestQueryHandler : IQueryHandler<GetApprenticeshipRequest, GetApprenticeshipResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipsRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipResponse?> Handle(GetApprenticeshipRequest query, CancellationToken cancellationToken = default)
    {
        var apprenticeship = await _apprenticeshipQueryRepository.GetApprenticeshipWithEpisodes(query.Ukprn, query.Uln);

        if (apprenticeship == null) return null;

        return new GetApprenticeshipResponse(query.Ukprn, apprenticeship);
    }
}