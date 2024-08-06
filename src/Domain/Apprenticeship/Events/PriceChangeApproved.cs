using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events
{
    public class PriceChangeApproved : IDomainEvent
    {
        public Guid ApprenticeshipKey { get; }
        public Guid PriceHistoryKey { get; }
        public ApprovedBy ApprovedBy { get; }
        public PriceChangeApproved(Guid apprenticeshipKey, Guid priceHistoryKey, ApprovedBy approvedBy)
        {
            ApprenticeshipKey = apprenticeshipKey;
            PriceHistoryKey = priceHistoryKey;
            ApprovedBy = approvedBy;
        }
    }
}
