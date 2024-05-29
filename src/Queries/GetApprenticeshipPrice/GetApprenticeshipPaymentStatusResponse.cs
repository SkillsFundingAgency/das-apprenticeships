namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;

public class GetApprenticeshipPaymentStatusResponse
{
	public Guid ApprenticeshipKey { get; set; }
	public bool? PaymentsFrozen { get; set; }
}