namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models
{
    public class PriceHistoryModel
    {
        public decimal TrainingPrice { get; set; }
        public decimal AssessmentPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}
