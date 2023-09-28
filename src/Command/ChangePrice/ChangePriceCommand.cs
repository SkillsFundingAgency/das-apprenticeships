namespace SFA.DAS.Apprenticeships.Command.ChangePrice
{
    public class ChangePriceCommand : ICommand
    {
        public long ApprovalsApprenticeshipId { get; set; }
        public decimal TrainingPrice { get; set; }
        public decimal AssessmentPrice { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime ApprovedDate { get; set; }
        public long EmployerAccountId { get; set; }
    }
}
