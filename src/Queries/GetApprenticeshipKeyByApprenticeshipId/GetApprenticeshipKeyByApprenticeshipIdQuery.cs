using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;

public class GetApprenticeshipKeyRequestQueryHandler : IQueryHandler<GetApprenticeshipKeyByApprenticeshipIdRequest, GetApprenticeshipKeyByApprenticeshipIdResponse>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipKeyRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipKeyByApprenticeshipIdResponse> Handle(GetApprenticeshipKeyByApprenticeshipIdRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _apprenticeshipQueryRepository.GetKeyByApprenticeshipId(query.ApprenticeshipId);
        return new GetApprenticeshipKeyByApprenticeshipIdResponse { ApprenticeshipKey = key };
    }
}