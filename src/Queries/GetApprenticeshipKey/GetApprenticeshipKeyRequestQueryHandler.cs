using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;

public class GetApprenticeshipKeyRequestQueryHandler : IQueryHandler<GetApprenticeshipKeyRequest, GetApprenticeshipKeyResponse>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipKeyRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipKeyResponse> Handle(GetApprenticeshipKeyRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _apprenticeshipQueryRepository.GetKey(query.ApprenticeshipHashedId);
        return new GetApprenticeshipKeyResponse { ApprenticeshipKey = key };
    }
}