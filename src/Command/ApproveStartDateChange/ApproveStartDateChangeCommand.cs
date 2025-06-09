namespace SFA.DAS.Learning.Command.ApproveStartDateChange;

public class ApproveStartDateChangeCommand : ICommand
{
    public ApproveStartDateChangeCommand(Guid apprenticeshipKey, string userId)
    {
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;
    }

    public Guid ApprenticeshipKey { get; set; }
    public string UserId { get; set; }
}