namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models
{
    public class ApprenticeshipModel
    {
        public Guid Key { get; internal set; }
        public string Uln { get; set; }
        public string TrainingCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<ApprovalModel> Approvals { get; set; }

        public ApprenticeshipModel()
        {
            Approvals = new List<ApprovalModel>();
        }

        public ApprenticeshipModel(Guid key) : this()
        {
            Key = key;
        }
    }
}
