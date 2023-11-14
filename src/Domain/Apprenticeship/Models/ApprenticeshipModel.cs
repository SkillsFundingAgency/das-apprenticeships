namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models
{
    public class ApprenticeshipModel
    {
        public Guid Key { get; internal set; }
        public string Uln { get; set; }
        public string TrainingCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public decimal? TrainingPrice { get; set; }
        public decimal? EndPointAssessmentPrice { get; set; }
        public decimal TotalPrice { get; set; }
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
