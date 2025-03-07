using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

[ExcludeFromCodeCoverage]
public class HandleWithdrawalNotificationsRequest
{
    public DateTime LastDayOfLearning { get; set; }
    public string Reason { get; set; }
}