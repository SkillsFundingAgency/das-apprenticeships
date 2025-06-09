using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetApprenticeshipStartDate;

public class GetApprenticeshipStartDateQueryHandler : IQueryHandler<GetApprenticeshipStartDateRequest, GetApprenticeshipStartDateResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipStartDateQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipStartDateResponse?> Handle(GetApprenticeshipStartDateRequest query, CancellationToken cancellationToken = default)
    {
        var startDate = await _apprenticeshipQueryRepository.GetStartDate(query.ApprenticeshipKey);

        return new GetApprenticeshipStartDateResponse
        {
            ApprenticeshipStartDate = startDate
        };
    }
}