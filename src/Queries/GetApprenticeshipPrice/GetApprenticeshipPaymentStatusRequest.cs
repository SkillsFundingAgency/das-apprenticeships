namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;

public class GetApprenticeshipPaymentStatusRequest : IQuery
{
	public Guid ApprenticeshipKey { get; set; }
}