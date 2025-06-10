using SFA.DAS.Learning.Domain.Repositories;

namespace SFA.DAS.Learning.Queries.GetApprenticeshipPaymentStatus;

public class GetLearningPaymentStatusQueryHandler : IQueryHandler<GetLearningPaymentStatusRequest, GetLearningPaymentStatusResponse?>
{
    private readonly ILearningQueryRepository _learningQueryRepository;

    public GetLearningPaymentStatusQueryHandler(ILearningQueryRepository learningQueryRepository)
    {
        _learningQueryRepository = learningQueryRepository;
    }

    public async Task<GetLearningPaymentStatusResponse?> Handle(GetLearningPaymentStatusRequest query, CancellationToken cancellationToken = default)
    {
        var paymentsFrozen = await _learningQueryRepository.GetPaymentStatus(query.LearningKey);

        if(paymentsFrozen == null)
            return null;

        return new GetLearningPaymentStatusResponse
        {
            LearningKey = query.LearningKey,
            PaymentsFrozen = paymentsFrozen.IsFrozen,
            ReasonFrozen = paymentsFrozen.Reason,
            FrozenOn = paymentsFrozen.FrozenOn
        };
    }
}