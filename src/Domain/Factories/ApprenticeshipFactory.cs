using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public class ApprenticeshipFactory : IApprenticeshipFactory
    {
        public Apprenticeship.Apprenticeship CreateNew(string uln, string trainingCode)
        {
            return Apprenticeship.Apprenticeship.New(uln, trainingCode);
        }

        public Apprenticeship.Apprenticeship GetExisting(ApprenticeshipModel model)
        {
            return Apprenticeship.Apprenticeship.Get(model);
        }
    }
}
