using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class PriceHistory : ValueObject
    {
        private readonly PriceHistoryModel _model;
        public decimal TrainingPrice => _model.TrainingPrice;
        public decimal AssessmentPrice => _model.AssessmentPrice;
        public decimal TotalPrice => _model.TotalPrice;
        public DateTime EffectiveFrom => _model.EffectiveFrom;
        public DateTime ApprovedDate => _model.ApprovedDate;

        internal static PriceHistory New(DateTime approvedDate, decimal assessmentPrice, decimal trainingPrice, DateTime effectiveFromDate)
        {
            return new PriceHistory(new PriceHistoryModel
            {
                ApprovedDate = approvedDate,
                AssessmentPrice = assessmentPrice,
                TrainingPrice = trainingPrice,
                EffectiveFrom = effectiveFromDate,
                TotalPrice = trainingPrice + assessmentPrice
            });
        }

        private PriceHistory(PriceHistoryModel model)
        {
            _model = model;
        }

        public PriceHistoryModel GetModel()
        {
            return _model;
        }

        internal static PriceHistory Get(PriceHistoryModel model)
        {
            return new PriceHistory(model);
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ApprovedDate;
            yield return AssessmentPrice;
            yield return TrainingPrice;
            yield return EffectiveFrom;
            yield return TotalPrice;
        }
    }
}
