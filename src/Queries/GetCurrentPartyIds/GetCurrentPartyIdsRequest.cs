namespace SFA.DAS.Learning.Queries.GetCurrentPartyIds;

public class GetCurrentPartyIdsRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}