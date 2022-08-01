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
                Approvals = domainModel.Approvals.Map(domainModel.Key)
            };

            return dataModel;
        }

        internal static ApprenticeshipModel Map(this Entities.Apprenticeship.Apprenticeship dataModel)
        {
            var domainModel = new ApprenticeshipModel(dataModel.Key)
            {
                TrainingCode = dataModel.TrainingCode,
                Uln = dataModel.Uln
            };

            domainModel.Approvals = dataModel.Approvals.Map();

            return domainModel;
        }
    }
}
