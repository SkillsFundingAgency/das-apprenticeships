using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetPendingStartDateChange;

public class GetPendingStartDateChangeQueryHandler : IQueryHandler<GetPendingStartDateChangeRequest, GetPendingStartDateChangeResponse>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetPendingStartDateChangeQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetPendingStartDateChangeResponse> Handle(GetPendingStartDateChangeRequest query, CancellationToken cancellationToken = default)
    {
        var pendingStartDateChange = await _apprenticeshipQueryRepository.GetPendingStartDateChange(query.ApprenticeshipKey);

        if (pendingStartDateChange == null) return new GetPendingStartDateChangeResponse { HasPendingStartDateChange = false };

        return new GetPendingStartDateChangeResponse
        {
            HasPendingStartDateChange = true,
            PendingStartDateChange = pendingStartDateChange
        };
    }
}