namespace SFA.DAS.Learning.Types;

public abstract class LearningEvent
{
    public LearningEpisode Episode { get; set; }
    public Guid LearningKey { get; set; }
}