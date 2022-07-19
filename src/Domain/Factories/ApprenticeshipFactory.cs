namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public Apprenticeship.Apprenticeship CreateNew()
        {
            return Apprenticeship.Apprenticeship.New();
        }
    }
}
