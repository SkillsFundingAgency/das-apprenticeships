using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;
using PriceHistory = SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship.PriceHistory;

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
            PriceChangeRequestStatus = domainModel.PriceChangeRequestStatus.ToString()
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
            CreatedDate = dataModel.CreatedDate
        };

        if (dataModel.PriceChangeRequestStatus != null && Enum.TryParse(dataModel.PriceChangeRequestStatus, out PriceChangeRequestStatus priceChangeRequestStatus))
            domainModel.PriceChangeRequestStatus = priceChangeRequestStatus;

        return domainModel;
    }
}