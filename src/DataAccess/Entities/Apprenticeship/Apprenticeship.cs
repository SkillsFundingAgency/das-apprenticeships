namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship
{
    [Table("dbo.Apprenticeship")]
    [System.ComponentModel.DataAnnotations.Schema.Table("Apprenticeship")]
    public class Apprenticeship
    {
        [Key]
        public Guid Key { get; set; }
        public string Uln { get; set; }
        public string TrainingCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public decimal? TrainingPrice { get; set; }
        public decimal? EndPointAssessmentPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int? FundingBandMaximum { get; set; }
        public List<Approval> Approvals { get; set; }
        public List<PriceHistory> PriceHistories { get; set; }
    }
}