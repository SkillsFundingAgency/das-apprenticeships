namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

public class PriceHistoryModel
{
    public Guid Key { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public long ApprenticeshipId { get; set; }
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