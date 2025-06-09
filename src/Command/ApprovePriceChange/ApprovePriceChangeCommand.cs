namespace SFA.DAS.Learning.Command.ApprovePriceChange;

public class ApprovePriceChangeCommand : ICommand
{
    public ApprovePriceChangeCommand(Guid apprenticeshipKey, string userId, decimal? trainingPrice, decimal? assessmentPrice)
    {
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;
        TrainingPrice = trainingPrice;
        AssessmentPrice = assessmentPrice;
    }

    public Guid ApprenticeshipKey { get; set; }
    public string UserId { get; set; }
    public decimal? TrainingPrice { get; set; }// Only used when a provider is approving a employer initiated price change
    public decimal? AssessmentPrice { get; set; }// Only used when a provider is approving a employer initiated price change
}