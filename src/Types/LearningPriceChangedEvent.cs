using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Types;

public class LearningPriceChangedEvent : LearningEvent
{
    public long LearningId { get; set; }
    public DateTime EffectiveFromDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public ApprovedBy ApprovedBy { get; set; }
}