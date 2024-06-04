namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPaymentStatus;

public class GetApprenticeshipPaymentStatusResponse
{
    public Guid ApprenticeshipKey { get; set; }
    public bool? PaymentsFrozen { get; set; }
}