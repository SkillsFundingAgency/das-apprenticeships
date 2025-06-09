using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetApprenticeshipPaymentStatus;

public class GetApprenticeshipPaymentStatusQueryHandler : IQueryHandler<GetApprenticeshipPaymentStatusRequest, GetApprenticeshipPaymentStatusResponse?>
{
    private readonly IApprenticeshipQueryRepository _apprenticeshipQueryRepository;

    public GetApprenticeshipPaymentStatusQueryHandler(IApprenticeshipQueryRepository apprenticeshipQueryRepository)
    {
        _apprenticeshipQueryRepository = apprenticeshipQueryRepository;
    }

    public async Task<GetApprenticeshipPaymentStatusResponse?> Handle(GetApprenticeshipPaymentStatusRequest query, CancellationToken cancellationToken = default)
    {
        var paymentsFrozen = await _apprenticeshipQueryRepository.GetPaymentStatus(query.ApprenticeshipKey);

        if(paymentsFrozen == null)
            return null;

        return new GetApprenticeshipPaymentStatusResponse
        {
            ApprenticeshipKey = query.ApprenticeshipKey,
            PaymentsFrozen = paymentsFrozen.IsFrozen,
            ReasonFrozen = paymentsFrozen.Reason,
            FrozenOn = paymentsFrozen.FrozenOn
        };
    }
}