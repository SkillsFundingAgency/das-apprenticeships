namespace SFA.DAS.Apprenticeships.Queries.GetLearnerStatus;

public class GetLearnerStatusRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}