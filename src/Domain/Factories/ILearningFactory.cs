using SFA.DAS.Learning.Domain.Apprenticeship;

namespace SFA.DAS.Learning.Domain.Factories
{
    public interface ILearningFactory
    {
        ApprenticeshipDomainModel CreateNew(
            long approvalsApprenticeshipId,
            string uln, 
            DateTime dateOfBirth,
            string firstName, 
            string lastName, 
            string apprenticeshipHashedId);
        
        ApprenticeshipDomainModel GetExisting(DataAccess.Entities.Apprenticeship.Apprenticeship model);
    }
}
