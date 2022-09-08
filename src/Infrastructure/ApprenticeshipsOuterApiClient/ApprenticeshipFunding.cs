namespace SFA.DAS.Apprenticeships.Infrastructure.ApprenticeshipsOuterApiClient;

public class ApprenticeshipFunding
{
    public int MaxEmployerLevyCap { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public int Duration { get; set; }
}