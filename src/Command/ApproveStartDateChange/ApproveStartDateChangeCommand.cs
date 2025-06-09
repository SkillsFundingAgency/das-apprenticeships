namespace SFA.DAS.Learning.Command.ApproveStartDateChange;

public class ApproveStartDateChangeCommand : ICommand
{
    public ApproveStartDateChangeCommand(Guid learningKey, string userId)
    {
        LearningKey = learningKey;
        UserId = userId;
    }

    public Guid LearningKey { get; set; }
    public string UserId { get; set; }
}