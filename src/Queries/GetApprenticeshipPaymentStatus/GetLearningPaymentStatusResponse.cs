namespace SFA.DAS.Learning.Queries.GetApprenticeshipPaymentStatus;

public class GetLearningPaymentStatusResponse
{
    public Guid LearningKey { get; set; }
    public Guid ApprenticeshipKey => LearningKey;
    public bool? PaymentsFrozen { get; set; }
    public string? ReasonFrozen { get; set; }
    public DateTime? FrozenOn { get; set; }
}