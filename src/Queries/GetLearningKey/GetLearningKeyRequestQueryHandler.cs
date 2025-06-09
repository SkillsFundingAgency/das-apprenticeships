using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetLearningKey;

public class GetLearningKeyRequestQueryHandler : IQueryHandler<GetLearningKeyRequest, GetLearningKeyResponse>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetLearningKeyRequestQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetLearningKeyResponse> Handle(GetLearningKeyRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _apprenticeshipQueryRepository.GetKey(query.ApprenticeshipHashedId);
        return new GetLearningKeyResponse { ApprenticeshipKey = key };
    }
}