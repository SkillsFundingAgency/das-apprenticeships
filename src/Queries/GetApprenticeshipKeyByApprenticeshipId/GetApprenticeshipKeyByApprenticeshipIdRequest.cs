namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKeyByApprenticeshipId;

public class GetApprenticeshipKeyByApprenticeshipIdRequest : IQuery
{
    public long ApprenticeshipId { get; set; }
}