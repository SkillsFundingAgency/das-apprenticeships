using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Types;

public class ApprenticeshipPriceChangedEvent : ApprenticeshipEvent
{
    public long ApprenticeshipId { get; set; }
    public DateTime EffectiveFromDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public ApprovedBy ApprovedBy { get; set; }
}