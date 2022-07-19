namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public Apprenticeship.Apprenticeship CreateNew(long uln, string trainingCode)
        {
            return Apprenticeship.Apprenticeship.New(uln, trainingCode);
        }
    }
}
