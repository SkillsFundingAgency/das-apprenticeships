using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Types;

#pragma warning disable CS8618
public class ApprenticeshipPriceChangedEvent : ApprenticeshipEvent
{
    public long ApprenticeshipId { get; set; }
    public DateTime EffectiveFromDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public ApprovedBy ApprovedBy { get; set; }
}
#pragma warning restore CS8618