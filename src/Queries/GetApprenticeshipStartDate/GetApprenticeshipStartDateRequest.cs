namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipStartDate;

public class GetApprenticeshipStartDateRequest : IQuery
{
    public Guid ApprenticeshipKey { get; set; }
}
