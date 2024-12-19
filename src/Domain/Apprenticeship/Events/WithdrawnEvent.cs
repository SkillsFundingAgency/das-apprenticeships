namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class WithdrawnEvent : IDomainEvent
{
    public Guid ApprenticeshipKey { get; }
    public long ApprenticeshipId { get; }
    public string Reason { get; }
    public DateTime LastDayOfLearning { get; }

    public WithdrawnEvent(Guid apprenticeshipKey, long apprenticeshipId, string reason, DateTime lastDayOfLearning)
    {
        ApprenticeshipKey = apprenticeshipKey;
        ApprenticeshipId = apprenticeshipId;
        Reason = reason;
        LastDayOfLearning = lastDayOfLearning;
    }
}
