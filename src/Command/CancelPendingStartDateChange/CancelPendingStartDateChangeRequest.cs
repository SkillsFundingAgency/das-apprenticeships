namespace SFA.DAS.Apprenticeships.Command.CancelPendingStartDateChange;

public class CancelPendingStartDateChangeRequest : ICommand
{
	public CancelPendingStartDateChangeRequest(Guid apprenticeshipKey)
	{
		ApprenticeshipKey = apprenticeshipKey;
	}

	public Guid ApprenticeshipKey { get; set; }
}