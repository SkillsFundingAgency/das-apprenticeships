using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetLearningKey;

public class GetLearningKeyRequestQueryHandler : IQueryHandler<GetLearningKeyRequest, GetLearningKeyResponse>
{
    private readonly ILearningQueryRepository _learningQueryRepository;

    public GetLearningKeyRequestQueryHandler(ILearningQueryRepository learningQueryRepository)
    {
        _learningQueryRepository = learningQueryRepository;
    }

    public async Task<GetLearningKeyResponse> Handle(GetLearningKeyRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _learningQueryRepository.GetKey(query.ApprenticeshipHashedId);
        return new GetLearningKeyResponse { LearningKey = key };
    }
}