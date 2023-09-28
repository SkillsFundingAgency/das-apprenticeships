namespace SFA.DAS.Apprenticeships.Types
{
    public class ApprenticeshipPriceChangedEvent
    {
        public Guid ApprenticeshipKey { get; set; }
        public decimal TrainingPrice { get; set; }
        public decimal AssessmentPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}
