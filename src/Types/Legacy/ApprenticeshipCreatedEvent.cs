using SFA.DAS.Learning.Types;

namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipCreatedEvent : ApprenticeshipEvent
{
    public long ApprovalsApprenticeshipId { get; set; }

    public string Uln { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public static implicit operator ApprenticeshipCreatedEvent(LearningCreatedEvent learningCreatedEvent)
    {
        return new ApprenticeshipCreatedEvent
        {
            ApprovalsApprenticeshipId = learningCreatedEvent.ApprovalsApprenticeshipId,
            ApprenticeshipKey = learningCreatedEvent.LearningKey,
            DateOfBirth = learningCreatedEvent.DateOfBirth,
            Episode = learningCreatedEvent.Episode
        };
    }
}