namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipKey;

public class GetApprenticeshipKeyRequest : IQuery
{
    public string ApprenticeshipHashedId { get; set; }
}