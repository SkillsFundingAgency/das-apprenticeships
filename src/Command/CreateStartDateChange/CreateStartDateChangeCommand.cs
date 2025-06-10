namespace SFA.DAS.Learning.Command.CreateStartDateChange;

public class CreateStartDateChangeCommand : ICommand
{
    public CreateStartDateChangeCommand(
		string initiator,
		Guid apprenticeshipKey,
        string userId, 
        DateTime actualStartDate,
        DateTime plannedEndDate,
        string reason)
    {
        Initiator = initiator;
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;
        ActualStartDate = actualStartDate;
        PlannedEndDate = plannedEndDate;
        Reason = reason;
    }
    public string Initiator { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public string UserId { get; set; }
    public DateTime ActualStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public string Reason { get; set; }
}