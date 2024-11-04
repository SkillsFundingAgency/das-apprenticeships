namespace SFA.DAS.Apprenticeships.Queries.GetCurrentPartyIds;

public class GetCurrentPartyIdsRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}