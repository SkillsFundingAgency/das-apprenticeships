using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Types;

public class PriceChangeApprovedEvent
{
    public Guid ApprenticeshipKey { get; set; }
    public long ApprenticeshipId { get; set; }
    public decimal TrainingPrice { get; set; }
    public decimal AssessmentPrice { get; set; }
    public DateTime EffectiveFromDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public ApprovedBy ApprovedBy { get; set; }
    public long EmployerAccountId { get; set; }
    public long ProviderId { get; set; }

    public Guid ApprenticeshipEpisodeKey { get; set; }
    public Guid PriceKey { get; set; }
    public List<Guid> DeletedPriceKeys { get; set; }
}
