using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

[Table("dbo.StartDateChange")]
[System.ComponentModel.DataAnnotations.Schema.Table("StartDateChange")]
public class StartDateChange
{
    [Key]
    public Guid Key { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public DateTime ActualStartDate { get; set; }
	public DateTime PlannedEndDate { get; set; }
	public string Reason { get; set; } = null!;
    public string? ProviderApprovedBy { get; set; }
    public DateTime? ProviderApprovedDate { get; set; }
    public string? EmployerApprovedBy { get; set; }
    public DateTime? EmployerApprovedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public ChangeRequestStatus RequestStatus { get; set; }
    public ChangeInitiator? Initiator { get; set; }

}