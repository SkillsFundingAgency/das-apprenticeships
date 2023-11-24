using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.DataAccess.Mappers.Apprenticeship
{
    internal static class ApprenticeshipMapper
    {
        internal static Entities.Apprenticeship.Apprenticeship Map(this ApprenticeshipModel domainModel)
        {
            var dataModel = new Entities.Apprenticeship.Apprenticeship
            {
                Key = domainModel.Key,
                Uln = domainModel.Uln,
                TrainingCode = domainModel.TrainingCode,
                FirstName = domainModel.FirstName,
                LastName = domainModel.LastName,
                DateOfBirth = domainModel.DateOfBirth,
                ApprenticeshipHashedId = domainModel.ApprenticeshipHashedId,
                TrainingPrice = domainModel.TrainingPrice,
                EndPointAssessmentPrice = domainModel.EndPointAssessmentPrice,
                TotalPrice = domainModel.TotalPrice,
                FundingBandMaximum = domainModel.FundingBandMaximum,
                ActualStartDate = domainModel.ActualStartDate,
                PlannedEndDate = domainModel.PlannedEndDate,
                AccountLegalEntityId = domainModel.AccountLegalEntityId,
                Approvals = domainModel.Approvals.Map(domainModel.Key)
            };

            return dataModel;
        }

        internal static ApprenticeshipModel Map(this Entities.Apprenticeship.Apprenticeship dataModel)
        {
            var domainModel = new ApprenticeshipModel(dataModel.Key)
            {
                TrainingCode = dataModel.TrainingCode,
                Uln = dataModel.Uln,
                FirstName = dataModel.FirstName,
                LastName = dataModel.LastName,
                DateOfBirth = dataModel.DateOfBirth,
                ApprenticeshipHashedId = dataModel.ApprenticeshipHashedId,
                TrainingPrice = dataModel.TrainingPrice,
                EndPointAssessmentPrice = dataModel.EndPointAssessmentPrice,
                TotalPrice = dataModel.TotalPrice,
                FundingBandMaximum = dataModel.FundingBandMaximum,
                ActualStartDate = dataModel.ActualStartDate,
                PlannedEndDate = dataModel.PlannedEndDate,
                AccountLegalEntityId= dataModel.AccountLegalEntityId,
            };

            domainModel.Approvals = dataModel.Approvals.Map();

            return domainModel;
        }
    }
}
