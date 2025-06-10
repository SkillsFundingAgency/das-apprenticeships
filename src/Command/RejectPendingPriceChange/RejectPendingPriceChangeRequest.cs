namespace SFA.DAS.Learning.Command.RejectPendingPriceChange;

public class RejectPendingPriceChangeRequest(Guid learningKey, string? reason) : ICommand
{
    public Guid LearningKey { get; } = learningKey;
    public string? Reason { get; } = reason;
}