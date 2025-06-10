namespace SFA.DAS.Learning.InnerApi.Responses;

/// <summary>
/// Response returned from a request to get the current provider and employer ids that the apprenticeship is associated with
/// </summary>
public class CurrentPartyIdsResponse
{
    /// <summary>
    /// Provider Id
    /// </summary>
    public long Ukprn { get; set; }

    /// <summary>
    /// Employer Id
    /// </summary>
    public long EmployerAccountId { get; set; }

    /// <summary>
    /// Learning Id in the commitments/approvals db
    /// </summary>
    public long ApprovalsApprenticeshipId { get; set; }
}
