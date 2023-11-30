namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public interface IApprenticeshipFactory
    {
        Apprenticeship.ApprenticeshipDomainModel CreateNew(string uln, string trainingCode, DateTime dateOfBirth, string firstName, string lastName, decimal? trainingPrice, decimal? endpointAssessmentPrice, decimal totalPrice, string apprenticeshipHashedId, int fundingBandMaximum);
        Apprenticeship.ApprenticeshipDomainModel GetExisting(DataAccess.Entities.Apprenticeship.Apprenticeship model);
    }
}
