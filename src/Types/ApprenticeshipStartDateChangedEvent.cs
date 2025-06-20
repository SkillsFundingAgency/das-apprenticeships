namespace SFA.DAS.Apprenticeship.Types;

public class ApprenticeshipStartDateChangedEvent : ApprenticeshipEvent
{
    public long ApprenticeshipId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public string ProviderApprovedBy { get; set; }
    public string EmployerApprovedBy { get; set; }
    public string Initiator { get; set; }
}