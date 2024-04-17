namespace SFA.DAS.Apprenticeships.Queries.GetPendingStartDateChange;

public class GetPendingStartDateChangeRequest : IQuery
{
    public GetPendingStartDateChangeRequest(Guid apprenticeshipKey)
    {
        ApprenticeshipKey = apprenticeshipKey;
    }

    public Guid ApprenticeshipKey { get; set; }
}