namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

[Table("dbo.Apprenticeship")]
[System.ComponentModel.DataAnnotations.Schema.Table("Apprenticeship")]
public class Apprenticeship
{
	public Apprenticeship()
	{
		PriceHistories = new List<PriceHistory>();
		StartDateChanges = new List<StartDateChange>();
		FreezeRequests = new List<FreezeRequest>();
        Episodes = new List<Episode>();
        WithdrawalRequests = new List<WithdrawalRequest>();
    }
        
	[Key]
	public Guid Key { get; set; }
    public long ApprovalsApprenticeshipId { get; set; }
	public string Uln { get; set; } = null!;
	public string FirstName { get; set; } = null!;
	public string LastName { get; set; } = null!;
	public DateTime DateOfBirth { get; set; }
	public string ApprenticeshipHashedId { get; set; } = null!;
    public List<PriceHistory> PriceHistories { get; set; }
    public List<StartDateChange> StartDateChanges { get; set; }
    public List<FreezeRequest> FreezeRequests { get; set; }
    public List<Episode> Episodes { get; set; }
    public List<WithdrawalRequest> WithdrawalRequests { get; set; }
}