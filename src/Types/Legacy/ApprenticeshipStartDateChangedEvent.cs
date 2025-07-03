using SFA.DAS.Learning.Types;

namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipStartDateChangedEvent : ApprenticeshipEvent
{
    public long ApprenticeshipId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime ApprovedDate { get; set; }

    public string ProviderApprovedBy { get; set; }

    public string EmployerApprovedBy { get; set; }

    public string Initiator { get; set; }

    public static implicit operator ApprenticeshipStartDateChangedEvent(LearningStartDateChangedEvent learningEvent)
    {
        return new ApprenticeshipStartDateChangedEvent
        {
            ApprenticeshipId = learningEvent.ApprovalsApprenticeshipId,
            StartDate = learningEvent.StartDate,
            ApprovedDate = learningEvent.ApprovedDate,
            ProviderApprovedBy = learningEvent.ProviderApprovedBy,
            EmployerApprovedBy = learningEvent.EmployerApprovedBy,
            Initiator = learningEvent.Initiator,
            ApprenticeshipKey = learningEvent.LearningKey,
            Episode = learningEvent.Episode
        };
    }
}