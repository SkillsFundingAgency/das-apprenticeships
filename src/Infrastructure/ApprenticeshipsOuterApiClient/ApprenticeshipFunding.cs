using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Learning.Infrastructure.ApprenticeshipsOuterApiClient;

[ExcludeFromCodeCoverage]
public class ApprenticeshipFunding
{
    public int MaxEmployerLevyCap { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public int Duration { get; set; }
}