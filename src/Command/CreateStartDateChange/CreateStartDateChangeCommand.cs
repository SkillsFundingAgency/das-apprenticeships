namespace SFA.DAS.Apprenticeships.Command.CreateStartDateChange;

public class CreateStartDateChangeCommand : ICommand
{
    public CreateStartDateChangeCommand(
		string initiator,
		Guid apprenticeshipKey,
        string userId, 
        DateTime actualStartDate,
        string reason)
    {
        Initiator = initiator;
        ApprenticeshipKey = apprenticeshipKey;
        UserId = userId;
        ActualStartDate = actualStartDate;
        Reason = reason;
    }
    public string Initiator { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public string UserId { get; set; }
    public DateTime ActualStartDate { get; set; }
    public string Reason { get; set; }
}