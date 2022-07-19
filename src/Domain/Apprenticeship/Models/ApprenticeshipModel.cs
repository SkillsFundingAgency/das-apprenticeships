namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models
{
    public class ApprenticeshipModel
    {
        public Guid Key { get; internal set; }
        public long Uln { get; set; }
        public string TrainingCode { get; set; }
        public List<ApprovalModel> Approvals { get; set; }
    }
}
