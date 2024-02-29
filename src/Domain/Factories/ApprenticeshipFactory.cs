
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public ApprenticeshipDomainModel CreateNew(
            string uln, 
            string trainingCode, 
            DateTime dateOfBirth,
            string firstName, 
            string lastName, 
            decimal? trainingPrice, 
            decimal? endpointAssessmentPrice,
            decimal totalPrice, 
            string apprenticeshipHashedId, 
            int fundingBandMaximum, 
            DateTime? actualStartDate,
            DateTime plannedEndDate,
            long accountLegalEntityId,
            long ukprn)
        {
            return ApprenticeshipDomainModel.New(
                uln,
                trainingCode,
                dateOfBirth,
                firstName,
                lastName,
                trainingPrice,
                endpointAssessmentPrice,
                totalPrice,
                apprenticeshipHashedId,
                fundingBandMaximum,
                actualStartDate,
                plannedEndDate,
                accountLegalEntityId,
                ukprn);
        }

        public ApprenticeshipDomainModel GetExisting(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
        {
            return ApprenticeshipDomainModel.Get(entity);
        }
    }
}
