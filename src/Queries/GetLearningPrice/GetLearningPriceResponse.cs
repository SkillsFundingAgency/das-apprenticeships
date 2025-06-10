namespace SFA.DAS.Learning.Queries.GetLearningPrice;

public class GetLearningPriceResponse
{
    public Guid ApprenticeshipKey => LearningKey;
    public Guid LearningKey { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal? TotalPrice { get; set; }
    public int? FundingBandMaximum { get; set; }
    public DateTime? ApprenticeshipActualStartDate { get; set; }
    public DateTime? ApprenticeshipPlannedEndDate { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public long UKPRN { get; set; }
}