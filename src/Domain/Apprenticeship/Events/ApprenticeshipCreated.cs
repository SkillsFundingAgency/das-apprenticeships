namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events
{
    public class ApprenticeshipCreated : IDomainEvent
    {
        public Guid ApprenticeshipKey { get; }

        public ApprenticeshipCreated(Guid apprenticeshipKey)
        {
            ApprenticeshipKey = apprenticeshipKey;
        }
    }
}
