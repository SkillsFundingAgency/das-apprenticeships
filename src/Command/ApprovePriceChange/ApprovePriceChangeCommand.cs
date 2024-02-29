namespace SFA.DAS.Apprenticeships.Command.ApprovePriceChange;

public class ApprovePriceChangeCommand : ICommand
{
    public ApprovePriceChangeCommand(Guid apprenticeshipKey, string userId)
    {
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;
    }

    public Guid ApprenticeshipKey { get; set; }
    public string UserId { get; set; }
}