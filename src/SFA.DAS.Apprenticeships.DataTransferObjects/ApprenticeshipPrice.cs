namespace SFA.DAS.Apprenticeships.DataTransferObjects;

public class ApprenticeshipPrice
{
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int? FundingBandMaximum { get; set; }
    public DateTime? ApprenticeshipActualStartDate { get; set; }
    public DateTime? ApprenticeshipPlannedEndDate { get; set; }
}