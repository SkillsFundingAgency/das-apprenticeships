using SFA.DAS.Apprenticeships.DataAccess.Mappers.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Repositories;

namespace SFA.DAS.Apprenticeships.DataAccess.Repositories
{
    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
        private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

        public ApprenticeshipRepository(Lazy<ApprenticeshipsDataContext> dbContext)
        {
            _lazyContext = dbContext;
        }

        public async Task Add(Apprenticeship apprenticeship)
        {
            var apprenticeshipDataModel = apprenticeship.GetModel().Map();
            await DbContext.AddAsync(apprenticeshipDataModel);
        }
    }
}
