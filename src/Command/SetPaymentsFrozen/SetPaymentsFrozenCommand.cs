namespace SFA.DAS.Apprenticeships.Command.SetPaymentsFrozen;

public class SetPaymentsFrozenCommand : ICommand
{
    public Guid ApprenticeshipKey { get; }
    public string UserId { get; }
    public bool NewPaymentsFrozenStatus { get; }

    public SetPaymentsFrozenCommand(Guid apprenticeshipKey, string userId, SetPaymentsFrozen setPaymentsFrozen)
    {
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;

        switch (setPaymentsFrozen)
        {
            case SetPaymentsFrozen.Freeze:
                NewPaymentsFrozenStatus = true;
                break;
            case SetPaymentsFrozen.Unfreeze:
                NewPaymentsFrozenStatus = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(setPaymentsFrozen), setPaymentsFrozen, null);
        }
    }
}

public enum SetPaymentsFrozen
{
    Freeze,
    Unfreeze
}
