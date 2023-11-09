using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.DataAccess.Mappers.Apprenticeship;

internal static class PriceHistoryMapper
{
    internal static PriceHistory Map(this PriceHistoryModel domainModel)
    {
        var dataModel = new PriceHistory
        {
            Key = domainModel.Key,
            ApprenticeshipKey = domainModel.ApprenticeshipKey,
            ApprenticeshipId = domainModel.ApprenticeshipId,
            TrainingPrice = domainModel.TrainingPrice,
            AssessmentPrice = domainModel.AssessmentPrice,
            TotalPrice = domainModel.TotalPrice,
            EffectiveFromDate = domainModel.EffectiveFromDate,
            ProviderApprovedBy = domainModel.ProviderApprovedBy,
            ProviderApprovedDate = domainModel.ProviderApprovedDate,
            EmployerApprovedBy = domainModel.EmployerApprovedBy,
            EmployerApprovedDate = domainModel.EmployerApprovedDate,
            CreatedDate = domainModel.CreatedDate,
            PriceChangeRequestStatus = domainModel.PriceChangeRequestStatus
        };

        return dataModel;
    }

    internal static PriceHistoryModel Map(this PriceHistory dataModel)
    {
        var domainModel = new PriceHistoryModel
        {
            Key = dataModel.Key,
            ApprenticeshipKey = dataModel.ApprenticeshipKey,
            ApprenticeshipId = dataModel.ApprenticeshipId,
            TrainingPrice = dataModel.TrainingPrice,
            AssessmentPrice = dataModel.AssessmentPrice,
            TotalPrice = dataModel.TotalPrice,
            EffectiveFromDate = dataModel.EffectiveFromDate,
            ProviderApprovedBy = dataModel.ProviderApprovedBy,
            ProviderApprovedDate = dataModel.ProviderApprovedDate,
            EmployerApprovedBy = dataModel.EmployerApprovedBy,
            EmployerApprovedDate = dataModel.EmployerApprovedDate,
            CreatedDate = dataModel.CreatedDate,
            PriceChangeRequestStatus = dataModel.PriceChangeRequestStatus
        };

        return domainModel;
    }
}