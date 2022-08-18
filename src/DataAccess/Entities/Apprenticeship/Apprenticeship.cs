namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship
{
    [Table("dbo.Apprenticeship")]
    public class Apprenticeship
    {
        [Key]
        public Guid Key { get; set; }
        public string Uln { get; set; }
        public string TrainingCode { get; set; }
        public List<Approval> Approvals { get; set; }
    }
}
