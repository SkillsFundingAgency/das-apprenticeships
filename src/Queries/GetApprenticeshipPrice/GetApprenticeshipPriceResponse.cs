using SFA.DAS.Apprenticeships.DataTransferObjects;

namespace SFA.DAS.Apprenticeships.Queries.GetApprenticeshipPrice;

public class GetApprenticeshipPriceResponse
{
    public Guid ApprenticeshipKey { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? AssessmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal FundingBandMaximum { get; set; } //todo retrieve from approvals
    public DateTime EarliestEffectiveDate { get; set; } //todo retrieve from new collection calendar api
}