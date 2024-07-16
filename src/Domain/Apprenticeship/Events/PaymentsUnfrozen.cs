namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class PaymentsUnfrozen : IDomainEvent
{
    public Guid ApprenticeshipKey { get; }

    public PaymentsUnfrozen(Guid apprenticeshipKey)
    {
        ApprenticeshipKey = apprenticeshipKey;
    }
}
