namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipCreatedEvent
{
    public Guid ApprenticeshipKey { get; set; }
    public long ApprovalsApprenticeshipId { get; set; }
    public string Uln { get; set; }
    public long UKPRN { get; set; }
    public long EmployerAccountId { get; set; }
    public string LegalEntityName { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public decimal AgreedPrice { get; set; }
    public string TrainingCode { get; set; }
    public long? FundingEmployerAccountId { get; set; }
    public FundingType FundingType { get; set; }
    public int FundingBandMaximum { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int? AgeAtStartOfApprenticeship { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public bool? IsOnFlexiPaymentPilot { get; set; }
}
