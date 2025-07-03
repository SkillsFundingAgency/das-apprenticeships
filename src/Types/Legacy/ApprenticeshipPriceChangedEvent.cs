using SFA.DAS.Learning.Enums;
using SFA.DAS.Learning.Types;

namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipPriceChangedEvent : ApprenticeshipEvent
{
    public long ApprenticeshipId { get; set; }

    public DateTime EffectiveFromDate { get; set; }

    public DateTime ApprovedDate { get; set; }

    public ApprovedBy ApprovedBy { get; set; }

    public static implicit operator ApprenticeshipPriceChangedEvent(LearningPriceChangedEvent learningPriceChangedEvent)
    {
        return new ApprenticeshipPriceChangedEvent
        {
            ApprenticeshipId = learningPriceChangedEvent.ApprovalsApprenticeshipId,
            EffectiveFromDate = learningPriceChangedEvent.EffectiveFromDate,
            ApprovedDate = learningPriceChangedEvent.ApprovedDate,
            ApprovedBy = learningPriceChangedEvent.ApprovedBy,
            ApprenticeshipKey = learningPriceChangedEvent.LearningKey,
            Episode = learningPriceChangedEvent.Episode
        };
    }
}