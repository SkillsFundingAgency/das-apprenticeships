namespace SFA.DAS.Apprenticeships.Types;

public abstract class ApprenticeshipEvent
{
    public ApprenticeshipEpisode Episode { get; set; }
    public Guid ApprenticeshipKey { get; set; }
}