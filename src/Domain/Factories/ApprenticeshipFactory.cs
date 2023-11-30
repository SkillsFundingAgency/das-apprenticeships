namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public Apprenticeship.ApprenticeshipDomainModel CreateNew(string uln, string trainingCode, DateTime dateOfBirth, string firstName, string lastName, decimal? trainingPrice, decimal? endpointAssessmentPrice, decimal totalPrice, string apprenticeshipHashedId, int fundingBandMaximum)
        {
            return Apprenticeship.ApprenticeshipDomainModel.New(uln, trainingCode, dateOfBirth, firstName, lastName, trainingPrice, endpointAssessmentPrice, totalPrice, apprenticeshipHashedId, fundingBandMaximum);
        }

        public Apprenticeship.ApprenticeshipDomainModel GetExisting(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
        {
            return Apprenticeship.ApprenticeshipDomainModel.Get(entity);
        }
    }
}
