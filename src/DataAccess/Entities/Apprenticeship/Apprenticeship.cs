namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship
{
    [Table("dbo.Apprenticeship")]
    [System.ComponentModel.DataAnnotations.Schema.Table("Apprenticeship")]
    public class Apprenticeship
    {
        public Apprenticeship()
        {
            Approvals = new List<Approval>();
            PriceHistories = new List<PriceHistory>();
            StartDateChanges = new List<StartDateChange>();
        }
        
        [Key]
        public Guid Key { get; set; }
        public string Uln { get; set; } = null!;
        public string TrainingCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string ApprenticeshipHashedId { get; set; } = null!;
        public decimal? TrainingPrice { get; set; }
        public decimal? EndPointAssessmentPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int? FundingBandMaximum { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public long EmployerAccountId { get; set; }
        public List<Approval> Approvals { get; set; }
        public long? AccountLegalEntityId { get; set; }
        public List<PriceHistory> PriceHistories { get; set; }
        public long Ukprn { get; set; }
        public List<StartDateChange> StartDateChanges { get; set; }
        public string? TrainingCourseVersion { get; set; }
    }
}