using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.Domain.Repositories
{
    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
        private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

        public ApprenticeshipRepository(Lazy<ApprenticeshipsDataContext> dbContext)
        {
            _lazyContext = dbContext;
        }

        public async Task Add(DataAccess.Entities.Apprenticeship.Apprenticeship apprenticeship)
        {
            await DbContext.AddAsync(apprenticeship);
            await DbContext.SaveChangesAsync();
        }

        public async Task<DataAccess.Entities.Apprenticeship.Apprenticeship> Get(Guid key)
        {
            return await DbContext.Apprenticeships
                .Include(x => x.Approvals)
                .Include(x => x.PriceHistories)
                .SingleAsync(x => x.Key == key);
        }

        public async Task Update(DataAccess.Entities.Apprenticeship.Apprenticeship apprenticeship)
        {
            DbContext.Update(apprenticeship);
            await DbContext.SaveChangesAsync();
        }
    }
}
