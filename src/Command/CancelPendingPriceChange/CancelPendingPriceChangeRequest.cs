namespace SFA.DAS.Learning.Command.CancelPendingPriceChange;

public class CancelPendingPriceChangeRequest : ICommand
{
    public CancelPendingPriceChangeRequest(Guid apprenticeshipKey)
    {
        ApprenticeshipKey = apprenticeshipKey;
    }

    public Guid ApprenticeshipKey { get; set; }
}