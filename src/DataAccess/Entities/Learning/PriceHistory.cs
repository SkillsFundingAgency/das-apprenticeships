using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.DataAccess.Entities.Learning;

[Table("dbo.PriceHistory")]
[System.ComponentModel.DataAnnotations.Schema.Table("PriceHistory")]
public class PriceHistory
{
    [Key]
    public Guid Key { get; set; }
    public Guid LearningKey { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime EffectiveFromDate { get; set; }
    public string? ProviderApprovedBy { get; set; }
    public DateTime? ProviderApprovedDate { get; set; }
    public string? EmployerApprovedBy { get; set; }
    public DateTime? EmployerApprovedDate { get; set; }
    public string? ChangeReason { get; set; }
    public DateTime CreatedDate { get; set; }
    public ChangeRequestStatus? PriceChangeRequestStatus { get; set; }
    public string? RejectReason { get; set; }
    public ChangeInitiator? Initiator { get; set; }
}