using SFA.DAS.Learning.Domain.Apprenticeship;

namespace SFA.DAS.Learning.Domain.Factories
{
    public interface IApprenticeshipFactory
    {
        ApprenticeshipDomainModel CreateNew(
            long approvalsApprenticeshipId,
            string uln, 
            DateTime dateOfBirth,
            string firstName, 
            string lastName, 
            string apprenticeshipHashedId);
        
        ApprenticeshipDomainModel GetExisting(Learning.DataAccess.Entities.Apprenticeship.Apprenticeship model);
    }
}
