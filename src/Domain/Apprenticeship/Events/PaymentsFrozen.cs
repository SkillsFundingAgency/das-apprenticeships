namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class PaymentsFrozen : IDomainEvent
{
    public Guid ApprenticeshipKey { get; }

    public PaymentsFrozen(Guid apprenticeshipKey)
    {
        ApprenticeshipKey = apprenticeshipKey;
    }
}
