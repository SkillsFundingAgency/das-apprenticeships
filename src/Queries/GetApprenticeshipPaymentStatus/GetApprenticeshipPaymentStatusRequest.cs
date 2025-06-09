namespace SFA.DAS.Learning.Queries.GetApprenticeshipPaymentStatus;

public class GetApprenticeshipPaymentStatusRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}