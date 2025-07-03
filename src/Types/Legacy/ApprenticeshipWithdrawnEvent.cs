using SFA.DAS.Learning.Types;

namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipWithdrawnEvent
{
    public Guid ApprenticeshipKey { get; set; }

    public long ApprenticeshipId { get; set; }

    public string Reason { get; set; }

    public DateTime LastDayOfLearning { get; set; }

    public static implicit operator ApprenticeshipWithdrawnEvent(LearningWithdrawnEvent learningEvent)
    {
        return new ApprenticeshipWithdrawnEvent
        {
            ApprenticeshipKey = learningEvent.LearningKey,
            ApprenticeshipId = learningEvent.ApprovalsApprenticeshipId,
            Reason = learningEvent.Reason,
            LastDayOfLearning = learningEvent.LastDayOfLearning
        };
    }
}