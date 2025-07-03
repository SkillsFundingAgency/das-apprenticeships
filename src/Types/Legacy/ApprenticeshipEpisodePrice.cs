using SFA.DAS.Learning.Types;

namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipEpisodePrice
{
    public Guid Key { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal? TrainingPrice { get; set; }

    public decimal? EndPointAssessmentPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public int FundingBandMaximum { get; set; }

    public static implicit operator ApprenticeshipEpisodePrice(LearningEpisodePrice learningEpisodePrice)
    {
        return new ApprenticeshipEpisodePrice
        {
            Key = learningEpisodePrice.Key,
            StartDate = learningEpisodePrice.StartDate,
            EndDate = learningEpisodePrice.EndDate,
            TrainingPrice = learningEpisodePrice.TrainingPrice,
            EndPointAssessmentPrice = learningEpisodePrice.EndPointAssessmentPrice,
            TotalPrice = learningEpisodePrice.TotalPrice,
            FundingBandMaximum = learningEpisodePrice.FundingBandMaximum
        };
    }
}