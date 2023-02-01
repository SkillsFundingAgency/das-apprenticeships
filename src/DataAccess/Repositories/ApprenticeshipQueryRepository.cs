using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataAccess.Repositories
{
    public class ApprenticeshipQueryRepository : IApprenticeshipQueryRepository
    {
        private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
        private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

        public ApprenticeshipQueryRepository(Lazy<ApprenticeshipsDataContext> dbContext)
        {
            _lazyContext = dbContext;
        }

        public async Task<IEnumerable<Apprenticeship>> GetAll(long ukprn, FundingPlatform? fundingPlatform)
        {
            var dataModels = await DbContext.Apprenticeships
                .Include(x => x.Approvals)
                .Where(x => x.Approvals.Any(x => x.UKPRN == ukprn && (fundingPlatform == null || x.FundingPlatform == fundingPlatform)))
                .ToListAsync();

            List<Apprenticeship> result = new();
            foreach (var apprenticeDataModel in dataModels)
            {
                result.Add(new Apprenticeship() { Uln = apprenticeDataModel.Uln, LastName = apprenticeDataModel.LastName, FirstName = apprenticeDataModel.FirstName });
            }

            return result;
        }
    }
}
