using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetApprenticeshipKeyByApprenticeshipId;

public class GetApprenticeshipKeyByApprenticeshipIdQueryHandler : IQueryHandler<GetApprenticeshipKeyByApprenticeshipIdRequest, GetApprenticeshipKeyByApprenticeshipIdResponse>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipKeyByApprenticeshipIdQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipKeyByApprenticeshipIdResponse> Handle(GetApprenticeshipKeyByApprenticeshipIdRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _apprenticeshipQueryRepository.GetKeyByApprenticeshipId(query.ApprenticeshipId);
        return new GetApprenticeshipKeyByApprenticeshipIdResponse { ApprenticeshipKey = key };
    }
}