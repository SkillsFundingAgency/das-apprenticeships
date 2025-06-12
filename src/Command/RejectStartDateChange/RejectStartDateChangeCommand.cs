namespace SFA.DAS.Learning.Command.RejectStartDateChange;

public class RejectStartDateChangeCommand : ICommand
{
    public RejectStartDateChangeCommand(Guid apprenticeshipKey, string? reason)
    {
        ApprenticeshipKey = apprenticeshipKey;
        Reason = reason;
    }

    public Guid ApprenticeshipKey { get; set; }
    public string? Reason { get; set; }
}