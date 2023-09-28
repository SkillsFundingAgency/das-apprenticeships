using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.DataAccess.Mappers.Apprenticeship;

internal static class PriceHistoryMapper
{
    internal static List<PriceHistory> Map(this List<PriceHistoryModel> domainModels, Guid apprenticeshipKey)
    {
        var dataModels = domainModels.Select(x => new PriceHistory
        {
            ApprenticeshipKey = apprenticeshipKey,
            AssessmentPrice = x.AssessmentPrice,
            ApprovedDate = x.ApprovedDate,
            EffectiveFrom = x.EffectiveFrom,
            TotalPrice = x.TotalPrice,
            TrainingPrice = x.TrainingPrice

        }).ToList();

        return dataModels;
    }        
        
    internal static List<PriceHistoryModel> Map(this List<PriceHistory> dataModels)
    {
        var domainModels = dataModels.Select(x => new PriceHistoryModel
        {
            AssessmentPrice = x.AssessmentPrice,
            ApprovedDate = x.ApprovedDate,
            EffectiveFrom = x.EffectiveFrom,
            TotalPrice = x.TotalPrice,
            TrainingPrice = x.TrainingPrice

        }).ToList();

        return domainModels;
    }
}