﻿using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;

public class GetApprenticeshipKeyByApprenticeshipIdQueryHandler : IQueryHandler<GetApprenticeshipKeyByApprenticeshipIdRequest, GetApprenticeshipKeyByApprenticeshipIdResponse>
{
    private readonly IApprovalQueryRepository _approvalQueryRepository;

    public GetApprenticeshipKeyByApprenticeshipIdQueryHandler(IApprovalQueryRepository apprenticeshipQueryRepository)
    {
        _approvalQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipKeyByApprenticeshipIdResponse> Handle(GetApprenticeshipKeyByApprenticeshipIdRequest query, CancellationToken cancellationToken = default)
    {
        var key = await _approvalQueryRepository.GetKeyByApprenticeshipId(query.ApprenticeshipId);
        return new GetApprenticeshipKeyByApprenticeshipIdResponse { ApprenticeshipKey = key };
    }
}