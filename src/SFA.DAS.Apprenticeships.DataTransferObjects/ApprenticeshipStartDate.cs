namespace SFA.DAS.Apprenticeships.DataTransferObjects;

public class ApprenticeshipStartDate
{
    public Guid ApprenticeshipKey { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public long UKPRN { get; set; }
}
