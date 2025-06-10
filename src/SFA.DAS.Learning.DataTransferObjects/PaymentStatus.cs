namespace SFA.DAS.Learning.DataTransferObjects;

public class PaymentStatus
{
    public bool IsFrozen { get; set; }
    public string? Reason { get; set; }
    public DateTime? FrozenOn { get; set; }
}
