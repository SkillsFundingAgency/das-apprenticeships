namespace SFA.DAS.Apprenticeships.Types;

public class ApprenticeshipCreatedEvent : EpisodeUpdatedEvent
{
    public Guid ApprenticeshipKey { get; set; }
    public long ApprovalsApprenticeshipId { get; set; }
    public string Uln { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}
