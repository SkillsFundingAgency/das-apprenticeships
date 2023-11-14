using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public Apprenticeship.Apprenticeship CreateNew(string uln, string trainingCode, DateTime dateOfBirth, string firstName, string lastName, decimal? trainingPrice, decimal? endpointAssessmentPrice, decimal totalPrice, string apprenticeshipHashedId)
        {
            return Apprenticeship.Apprenticeship.New(uln, trainingCode, dateOfBirth, firstName, lastName, trainingPrice, endpointAssessmentPrice, totalPrice, apprenticeshipHashedId);
        }

        public Apprenticeship.Apprenticeship GetExisting(ApprenticeshipModel model)
        {
            return Apprenticeship.Apprenticeship.Get(model);
        }
    }
}
