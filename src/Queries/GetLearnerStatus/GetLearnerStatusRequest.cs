namespace SFA.DAS.Learning.Queries.GetLearnerStatus;

public class GetLearnerStatusRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}