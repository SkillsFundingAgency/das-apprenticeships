using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetApprenticeshipStartDate;

public class GetLearningStartDateQueryHandler : IQueryHandler<GetLearningStartDateRequest, GetLearningStartDateResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetLearningStartDateQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetLearningStartDateResponse?> Handle(GetLearningStartDateRequest query, CancellationToken cancellationToken = default)
    {
        var startDate = await _apprenticeshipQueryRepository.GetStartDate(query.ApprenticeshipKey);

        return new GetLearningStartDateResponse
        {
            ApprenticeshipStartDate = startDate
        };
    }
}