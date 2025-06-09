namespace SFA.DAS.Learning.Queries.GetApprenticeshipPaymentStatus;

public class GetApprenticeshipPaymentStatusResponse
{
    public Guid ApprenticeshipKey { get; set; }
    public bool? PaymentsFrozen { get; set; }
    public string? ReasonFrozen { get; set; }
    public DateTime? FrozenOn { get; set; }
}