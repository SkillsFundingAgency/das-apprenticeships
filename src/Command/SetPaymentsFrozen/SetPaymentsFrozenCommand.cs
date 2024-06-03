namespace SFA.DAS.Apprenticeships.Command.SetPaymentsFrozen;

public class SetPaymentsFrozenCommand : ICommand
{
    public Guid ApprenticeshipKey { get; }
    public string UserId { get; }
    public bool NewPaymentsFrozenStatus { get; }

    public SetPaymentsFrozenCommand(Guid apprenticeshipKey, string userId, SetPayments setPaymentsFrozen)
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
    }
}

public enum SetPayments
{
    Freeze,
    Unfreeze
}
