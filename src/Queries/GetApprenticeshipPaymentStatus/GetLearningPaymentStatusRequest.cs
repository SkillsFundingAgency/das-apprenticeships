namespace SFA.DAS.Learning.Queries.GetApprenticeshipPaymentStatus;

public class GetLearningPaymentStatusRequest : IQuery
{
    public Guid LearningKey { get; set; }
}