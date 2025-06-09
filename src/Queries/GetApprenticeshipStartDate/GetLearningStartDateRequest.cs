namespace SFA.DAS.Learning.Queries.GetApprenticeshipStartDate;

public class GetLearningStartDateRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}
