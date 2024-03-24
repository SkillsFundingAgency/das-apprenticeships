using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;

public class GetApprenticeshipKeyRequestQueryHandler : IQueryHandler<GetApprenticeshipKeyByApprenticeshipIdRequest, GetApprenticeshipKeyByApprenticeshipIdResponse>
{
    private readonly IApprovalRepository _approvalRepository;

    public GetApprenticeshipKeyRequestQueryHandler(IApprovalRepository apprenticeshipQueryRepository)
    {
        _approvalRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipKeyByApprenticeshipIdResponse> Handle(GetApprenticeshipKeyByApprenticeshipIdRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _approvalRepository.GetKeyByApprenticeshipId(query.ApprenticeshipId);
        return new GetApprenticeshipKeyByApprenticeshipIdResponse { ApprenticeshipKey = key };
    }
}