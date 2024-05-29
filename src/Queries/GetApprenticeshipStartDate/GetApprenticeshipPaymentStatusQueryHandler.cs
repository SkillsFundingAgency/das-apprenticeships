using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipStartDate;

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

		return new GetApprenticeshipPaymentStatusResponse
		{
			PaymentsFrozen = paymentsFrozen
		};
	}
}