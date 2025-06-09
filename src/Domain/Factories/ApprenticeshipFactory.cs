using SFA.DAS.Learning.Domain.Apprenticeship;

namespace SFA.DAS.Learning.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public ApprenticeshipDomainModel CreateNew(
            long approvalsApprenticeshipId,
            string uln, 
            DateTime dateOfBirth,
            string firstName, 
            string lastName, 
            string apprenticeshipHashedId)
        {
            return ApprenticeshipDomainModel.New(
                approvalsApprenticeshipId,
                uln,
                dateOfBirth,
                firstName,
                lastName,
                apprenticeshipHashedId);
        }

        public ApprenticeshipDomainModel GetExisting(Learning.DataAccess.Entities.Apprenticeship.Apprenticeship entity)
        {
            return ApprenticeshipDomainModel.Get(entity);
        }
    }
}
