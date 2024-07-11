using SFA.DAS.Apprenticeships.Enums;
using static SFA.DAS.Apprenticeships.Domain.Apprenticeship.EpisodeDomainModel;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events
{
    public class PriceChangeApproved : IDomainEvent
    {
        public Guid ApprenticeshipKey { get; }
        public Guid PriceHistoryKey { get; }
        public ApprovedBy ApprovedBy { get; }
        public AmendedPrices AmendedPrices { get; set; }
        public PriceChangeApproved(Guid apprenticeshipKey, Guid priceHistoryKey, ApprovedBy approvedBy, AmendedPrices amendedPrices)
        {
            ApprenticeshipKey = apprenticeshipKey;
            PriceHistoryKey = priceHistoryKey;
            ApprovedBy = approvedBy;
            AmendedPrices = amendedPrices;
        }
    }
}
