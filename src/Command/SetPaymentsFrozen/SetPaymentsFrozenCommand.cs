namespace SFA.DAS.Learning.Command.SetPaymentsFrozen;

public class SetPaymentsFrozenCommand : ICommand
{
    public Guid LearningKey { get; }
    public string UserId { get; }
    public bool NewPaymentsFrozenStatus { get; }
    public string? Reason { get; set; }

    public SetPaymentsFrozenCommand(Guid learningKey, string userId, SetPayments setPaymentsFrozen, string? reason = null)
    {
        LearningKey = learningKey;
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
