using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Apprenticeships.DataAccess.Repositories
{
    public interface IApprenticeshipRepository
    {
        Task Add(Apprenticeship apprenticeship);
        Task<Apprenticeship> Get(Guid key);
        Task Update(Apprenticeship apprenticeship);
    }
    
    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
        private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

        public ApprenticeshipRepository(Lazy<ApprenticeshipsDataContext> dbContext)
        {
            _lazyContext = dbContext;
        }

        public async Task Add(Entities.Apprenticeship.Apprenticeship apprenticeship)
        {
            await DbContext.AddAsync(apprenticeship);
            await DbContext.SaveChangesAsync();
        }

        public async Task<Apprenticeship> Get(Guid key)
        {
            return await DbContext.Apprenticeships
                .Include(x => x.Approvals)
                .Include(x => x.PriceHistories)
                .SingleAsync(x => x.Key == key);
        }

        public async Task Update(Apprenticeship apprenticeship)
        {
            DbContext.Update(apprenticeship);
            await DbContext.SaveChangesAsync();
        }
    }
}
