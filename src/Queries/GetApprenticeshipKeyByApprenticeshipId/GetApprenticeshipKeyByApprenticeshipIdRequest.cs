namespace SFA.DAS.Learning.Queries.GetApprenticeshipKeyByApprenticeshipId;

public class GetApprenticeshipKeyByApprenticeshipIdRequest : IQuery
{
    public long ApprenticeshipId { get; set; }
}