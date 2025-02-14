using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain;

[ExcludeFromCodeCoverage]
public class LearnerStatusDetails
{
    public LearnerStatus LearnerStatus { get; set; }
    public DateTime? WithdrawalChangedDate { get; set; }
    public string? WithdrawalReason { get; set; }
    public DateTime? LastDayOfLearning { get; set; }
}