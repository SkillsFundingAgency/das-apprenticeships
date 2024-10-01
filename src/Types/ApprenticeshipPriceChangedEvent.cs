using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipPriceChangedEvent : ApprenticeshipEvent
{
    public long ApprenticeshipId { get; set; }
    public DateTime EffectiveFromDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public ApprovedBy ApprovedBy { get; set; }
}