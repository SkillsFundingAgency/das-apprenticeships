using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetLearningKeyByLearningId;

public class GetLearningKeyByLearningIdQueryHandler : IQueryHandler<GetLearningKeyByLearningIdRequest, GetLearningKeyByLearningIdResponse>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetLearningKeyByLearningIdQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetLearningKeyByLearningIdResponse> Handle(GetLearningKeyByLearningIdRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _apprenticeshipQueryRepository.GetKeyByApprenticeshipId(query.ApprenticeshipId);
        return new GetLearningKeyByLearningIdResponse { ApprenticeshipKey = key };
    }
}