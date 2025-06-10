using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetApprenticeshipStartDate;

public class GetLearningStartDateQueryHandler : IQueryHandler<GetLearningStartDateRequest, GetLearningStartDateResponse?>
{
    private readonly ILearningQueryRepository _learningQueryRepository;

    public GetLearningStartDateQueryHandler(ILearningQueryRepository learningQueryRepository)
    {
        _learningQueryRepository = learningQueryRepository;
    }

    public async Task<GetLearningStartDateResponse?> Handle(GetLearningStartDateRequest query, CancellationToken cancellationToken = default)
    {
        var startDate = await _learningQueryRepository.GetStartDate(query.ApprenticeshipKey);

        return new GetLearningStartDateResponse
        {
            LearningStartDate = startDate
        };
    }
}