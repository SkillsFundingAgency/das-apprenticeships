using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain;

[ExcludeFromCodeCoverage]
public class LearnerStatusWithWithdrawalChangedDate //todo name this something better?
{
    public LearnerStatus LearnerStatus { get; set; }
    public DateTime? WithdrawalChangedDate { get; set; }
}