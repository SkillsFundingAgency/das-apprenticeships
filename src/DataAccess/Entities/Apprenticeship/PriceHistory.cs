using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

[Table("dbo.PriceHistory")]
[System.ComponentModel.DataAnnotations.Schema.Table("PriceHistory")]
public class PriceHistory
{
    [Key]
    public Guid Key { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime EffectiveFromDate { get; set; }
    public string? ProviderApprovedBy { get; set; }
    public DateTime? ProviderApprovedDate { get; set; }
    public string? EmployerApprovedBy { get; set; }
    public DateTime? EmployerApprovedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public PriceChangeRequestStatus? PriceChangeRequestStatus { get; set; }
}