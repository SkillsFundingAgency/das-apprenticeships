namespace SFA.DAS.Learning.Command.CancelPendingPriceChange;

public class CancelPendingPriceChangeRequest(Guid learningKey) : ICommand
{
    public Guid LearningKey { get; set; } = learningKey;
}