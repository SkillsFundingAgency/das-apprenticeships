using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class PaymentStatus
{
    public bool IsFrozen { get; set; }
    public string? Reason { get; set; }
    public DateTime? FrozenOn { get; set; }
}
