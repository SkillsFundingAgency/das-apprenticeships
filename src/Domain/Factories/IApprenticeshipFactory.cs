using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public interface IApprenticeshipFactory
    {
        Apprenticeship.Apprenticeship CreateNew(
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
            long accountLegalEntityId);

        Apprenticeship.Apprenticeship GetExisting(ApprenticeshipModel model);
    }
}
