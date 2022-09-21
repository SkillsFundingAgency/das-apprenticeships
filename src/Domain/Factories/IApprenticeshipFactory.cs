using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.Domain.Factories
{
    public interface IApprenticeshipFactory
    {
        Apprenticeship.Apprenticeship CreateNew(string uln, string trainingCode);
        Apprenticeship.Apprenticeship GetExisting(ApprenticeshipModel model);
    }
}
