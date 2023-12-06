﻿
namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public Apprenticeship.Apprenticeship CreateNew(string uln, string trainingCode, DateTime dateOfBirth,
            string firstName, string lastName, decimal? trainingPrice, decimal? endpointAssessmentPrice,
            decimal totalPrice, string apprenticeshipHashedId, int fundingBandMaximum, DateTime? actualStartDate,
            DateTime plannedEndDate)
        {
            return Apprenticeship.Apprenticeship.New(uln, trainingCode, dateOfBirth, firstName, lastName, trainingPrice, endpointAssessmentPrice, totalPrice, apprenticeshipHashedId, fundingBandMaximum, actualStartDate, plannedEndDate);
        }

        public Apprenticeship.ApprenticeshipDomainModel GetExisting(DataAccess.Entities.Apprenticeship.Apprenticeship entity)
        {
            return Apprenticeship.ApprenticeshipDomainModel.Get(entity);
        }
    }
}
