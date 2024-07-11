using SFA.DAS.Apprenticeships.Enums;
using static SFA.DAS.Apprenticeships.Domain.Apprenticeship.EpisodeDomainModel;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class StartDateChangeApproved : IDomainEvent
{
    public Guid ApprenticeshipKey { get; }
    public Guid StartDateChangeKey { get; }
    public ApprovedBy ApprovedBy { get; }
    public AmendedPrices AmendedPrices { get; }

    public StartDateChangeApproved(Guid apprenticeshipKey, Guid startDateChangeKey, ApprovedBy approvedBy, AmendedPrices amendedPrices)
    {
        ApprenticeshipKey = apprenticeshipKey;
        StartDateChangeKey = startDateChangeKey;
        ApprovedBy = approvedBy;
        AmendedPrices = amendedPrices;
    }
}