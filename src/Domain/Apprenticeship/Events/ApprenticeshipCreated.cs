namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events
{
    public class ApprenticeshipCreated : IDomainEvent
    {
        private readonly Guid _apprenticeshipKey;

        public ApprenticeshipCreated(Guid apprenticeshipKey)
        {
            _apprenticeshipKey = apprenticeshipKey;
        }
    }
}
