
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public interface IApprenticeshipFactory
    {
        ApprenticeshipDomainModel CreateNew(
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
            long ukprn,
            long employerAccountId,
            string trainingCourseVersion,
            bool paymentsFrozen);
        
        ApprenticeshipDomainModel GetExisting(DataAccess.Entities.Apprenticeship.Apprenticeship model);

    }
}
