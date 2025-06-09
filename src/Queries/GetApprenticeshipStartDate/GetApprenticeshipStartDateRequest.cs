namespace SFA.DAS.Learning.Queries.GetApprenticeshipStartDate;

public class GetApprenticeshipStartDateRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}
