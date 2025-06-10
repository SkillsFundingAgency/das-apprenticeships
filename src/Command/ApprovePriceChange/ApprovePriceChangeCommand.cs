namespace SFA.DAS.Learning.Command.ApprovePriceChange;

public class ApprovePriceChangeCommand : ICommand
{
    public ApprovePriceChangeCommand(Guid learningKey, string userId, decimal? trainingPrice, decimal? assessmentPrice)
    {
        LearningKey = learningKey;
        UserId = userId;
        TrainingPrice = trainingPrice;
        AssessmentPrice = assessmentPrice;
    }

    public Guid LearningKey { get; set; }
    public string UserId { get; set; }
    public decimal? TrainingPrice { get; set; }// Only used when a provider is approving an employer initiated price change
    public decimal? AssessmentPrice { get; set; }// Only used when a provider is approving an employer initiated price change
}