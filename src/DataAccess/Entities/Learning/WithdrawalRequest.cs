namespace SFA.DAS.Learning.DataAccess.Entities.Learning;

[Table("dbo.WithdrawalRequest")]
[System.ComponentModel.DataAnnotations.Schema.Table("WithdrawalRequest")]
public class WithdrawalRequest
{
    [Key]
    public Guid Key { get; set; }
    public Guid ApprenticeshipKey { get; set; }
    public Guid EpisodeKey { get; set; }
    public string Reason { get; set; }
    public DateTime LastDayOfLearning { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? ProviderApprovedBy { get; set; }
}
