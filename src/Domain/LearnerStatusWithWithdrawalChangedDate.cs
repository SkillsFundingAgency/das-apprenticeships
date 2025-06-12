using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Learning.Domain.Apprenticeship;

namespace SFA.DAS.Learning.Domain;

[ExcludeFromCodeCoverage]
public class LearnerStatusDetails
{
    public LearnerStatus LearnerStatus { get; set; }
    public DateTime? WithdrawalChangedDate { get; set; }
    public string? WithdrawalReason { get; set; }
    public DateTime? LastDayOfLearning { get; set; }
}