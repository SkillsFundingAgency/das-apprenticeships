namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship
{
    public class Apprenticeship
    {
        public Guid Key { get; set; }
        public long Uln { get; set; }
        public string TrainingCode { get; set; }
        public List<Approval> Approvals { get; set; }
    }
}
