using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Events;

public class StartDateChangeApproved : IDomainEvent
{
    public Guid ApprenticeshipKey { get; }
    public Guid StartDateChangeKey { get; }
    public ApprovedBy ApprovedBy { get; }

    public StartDateChangeApproved(Guid apprenticeshipKey, Guid startDateChangeKey, ApprovedBy approvedBy)
    {
        ApprenticeshipKey = apprenticeshipKey;
        StartDateChangeKey = startDateChangeKey;
        ApprovedBy = approvedBy;
    }
}