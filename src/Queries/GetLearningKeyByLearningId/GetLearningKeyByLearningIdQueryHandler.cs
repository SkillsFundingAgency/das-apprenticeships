using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetLearningKeyByLearningId;

public class GetLearningKeyByLearningIdQueryHandler : IQueryHandler<GetLearningKeyByLearningIdRequest, GetLearningKeyByLearningIdResponse>
{
    private readonly ILearningQueryRepository _learningQueryRepository;

    public GetLearningKeyByLearningIdQueryHandler(ILearningQueryRepository learningQueryRepository)
    {
        _learningQueryRepository = learningQueryRepository;
    }

    public async Task<GetLearningKeyByLearningIdResponse> Handle(GetLearningKeyByLearningIdRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _learningQueryRepository.GetKeyByLearningId(query.ApprenticeshipId);
        return new GetLearningKeyByLearningIdResponse { LearningKey = key };
    }
}