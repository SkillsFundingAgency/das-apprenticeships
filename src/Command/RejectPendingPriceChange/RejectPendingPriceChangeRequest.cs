namespace SFA.DAS.Learning.Command.RejectPendingPriceChange;

public class RejectPendingPriceChangeRequest : ICommand
{
    public RejectPendingPriceChangeRequest(Guid apprenticeshipKey, string? reason)
    {
        ApprenticeshipKey = apprenticeshipKey;
        Reason = reason;
    }

    public Guid ApprenticeshipKey { get; }
    public string? Reason { get; }
}