using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<DataTransferObjects.Apprenticeship>> GetAll(long ukprn, FundingPlatform? fundingPlatform)
        {
            var dataModels = await DbContext.Apprenticeships
                .Include(x => x.Approvals)
                .Where(x => x.Approvals.Any(x => x.UKPRN == ukprn && (fundingPlatform == null || x.FundingPlatform == fundingPlatform)))
                .ToListAsync();

            var result = dataModels.Select(x => new DataTransferObjects.Apprenticeship { Uln = x.Uln, LastName = x.LastName, FirstName = x.FirstName });
            return result;
        }
    }
}