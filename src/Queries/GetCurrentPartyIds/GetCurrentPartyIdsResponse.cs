namespace SFA.DAS.Learning.Queries.GetCurrentPartyIds;

public class GetCurrentPartyIdsResponse
{
    public GetCurrentPartyIdsResponse(long ukprn, long employerAccountId, long approvalsApprenticeshipId)
    {
        Ukprn = ukprn;
        EmployerAccountId = employerAccountId;
        ApprovalsApprenticeshipId = approvalsApprenticeshipId;
    }

    public long Ukprn { get; set; }
    public long EmployerAccountId { get; set; }
    public long ApprovalsApprenticeshipId { get; set; }
}