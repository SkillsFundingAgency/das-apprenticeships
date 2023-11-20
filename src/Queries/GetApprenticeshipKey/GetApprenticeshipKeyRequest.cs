using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;

public class GetApprenticeshipKeyRequest : IQuery
{
    public string ApprenticeshipHashedId { get; set; }
}

public class GetApprenticeshipKeyResponse
{
    public Guid? ApprenticeshipKey { get; set; }
}

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