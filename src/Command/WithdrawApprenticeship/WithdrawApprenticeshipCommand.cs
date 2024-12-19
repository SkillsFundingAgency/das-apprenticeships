namespace SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;

public class WithdrawApprenticeshipCommand : ICommand
{
    public long UKPRN { get; set; }
    public string ULN { get; set; }
    public string Reason { get; set; }
    public string ReasonText { get; set; }
    public DateTime LastDayOfLearning { get; set; }
    public string ProviderApprovedBy { get; set; }
}

public enum WithdrawReason
{
    WithdrawFromStart,
    WithdrawDuringLearning,
    WithdrawFromBeta,
    Other
}
