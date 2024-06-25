
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public interface IApprenticeshipFactory
    {
        ApprenticeshipDomainModel CreateNew(
            string uln, 
            DateTime dateOfBirth,
            string firstName, 
            string lastName, 
            string apprenticeshipHashedId);
        
        ApprenticeshipDomainModel GetExisting(DataAccess.Entities.Apprenticeship.Apprenticeship model);
    }
}
