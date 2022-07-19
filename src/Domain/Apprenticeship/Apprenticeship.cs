using SFA.DAS.Apprenticeships.Domain.Apprenticeship.Models;

namespace SFA.DAS.Apprenticeships.Domain.Apprenticeship
{
    public class Apprenticeship
    {
        private readonly ApprenticeshipModel _model;

        internal static Apprenticeship New()
        {
            return new Apprenticeship(new ApprenticeshipModel { Key = Guid.NewGuid() });
        }

        private Apprenticeship(ApprenticeshipModel model)
        {
            _model = model;

        }

        public ApprenticeshipModel GetModel()
        {
            return _model;
        }
    }
}
