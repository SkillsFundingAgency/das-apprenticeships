namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPaymentStatus;

public class GetApprenticeshipPaymentStatusRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}