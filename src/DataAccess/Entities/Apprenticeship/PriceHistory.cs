using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship
{
    [Table("dbo.PriceHistory")]
    [System.ComponentModel.DataAnnotations.Schema.Table("PriceHistory")]
    [Owned]
    public class PriceHistory
    {
        public Guid ApprenticeshipKey { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime ApprovedDate { get; set; }

        public decimal TrainingPrice { get; set; }

        public decimal AssessmentPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
