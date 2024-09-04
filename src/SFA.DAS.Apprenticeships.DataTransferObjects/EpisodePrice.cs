using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class EpisodePrice
{
    public EpisodePrice(DateTime startDate, DateTime endDate, decimal? trainingPrice, decimal? endPointAssessmentPrice, decimal totalPrice, int fundingBandMaximum)
    {
        StartDate = startDate;
        EndDate = endDate;
        TrainingPrice = trainingPrice;
        EndPointAssessmentPrice = endPointAssessmentPrice;
        TotalPrice = totalPrice;
        FundingBandMaximum = fundingBandMaximum;
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? TrainingPrice { get; set; }
    public decimal? EndPointAssessmentPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int FundingBandMaximum { get; set; }
}