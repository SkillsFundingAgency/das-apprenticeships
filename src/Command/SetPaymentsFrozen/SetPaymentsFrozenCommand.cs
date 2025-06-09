namespace SFA.DAS.Learning.Command.SetPaymentsFrozen;

public class SetPaymentsFrozenCommand : ICommand
{
    public Guid ApprenticeshipKey { get; }
    public string UserId { get; }
    public bool NewPaymentsFrozenStatus { get; }
    public string? Reason { get; set; }

    public SetPaymentsFrozenCommand(Guid apprenticeshipKey, string userId, SetPayments setPaymentsFrozen, string? reason = null)
    {
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;

        switch (setPaymentsFrozen)
        {
            case SetPayments.Freeze:
                NewPaymentsFrozenStatus = true;
                break;
            case SetPayments.Unfreeze:
                NewPaymentsFrozenStatus = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(setPaymentsFrozen), setPaymentsFrozen, null);
        }
        Reason = reason;
    }
}

public enum SetPayments
{
    Freeze,
    Unfreeze
}
