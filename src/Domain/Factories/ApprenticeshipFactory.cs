
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public ApprenticeshipDomainModel CreateNew(
            string uln, 
            DateTime dateOfBirth,
            string firstName, 
            string lastName, 
            string apprenticeshipHashedId)
        {
            return ApprenticeshipDomainModel.New(
                uln,
                dateOfBirth,
                firstName,
                lastName,
                apprenticeshipHashedId);
        }

        public ApprenticeshipDomainModel GetExisting(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
        {
            return ApprenticeshipDomainModel.Get(entity);
        }
    }
}
