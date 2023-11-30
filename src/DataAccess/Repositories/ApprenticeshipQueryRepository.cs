using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.DataAccess.Repositories
{
    public interface IApprenticeshipQueryRepository
    {
        Task<IEnumerable<DataTransferObjects.Apprenticeship>> GetAll(long ukprn, FundingPlatform? fundingPlatform);
        Task<DataTransferObjects.ApprenticeshipPrice?> GetPrice(Guid apprenticeshipKey);
        Task<IEnumerable<DataTransferObjects.ApprenticeshipPrice>> GetPriceHistory(Guid apprenticeshipKey);
        Task<Guid?> GetKey(string apprenticeshipHashedId);
    }
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
                .Where(a => a.Approvals.Any(c => c.UKPRN == ukprn && (fundingPlatform == null || c.FundingPlatform == fundingPlatform)))
                .ToListAsync();
        
            var result = dataModels.Select(x => new DataTransferObjects.Apprenticeship { Uln = x.Uln, LastName = x.LastName, FirstName = x.FirstName });
            return result;
        }

        public async Task<ApprenticeshipPrice?> GetPrice(Guid apprenticeshipKey)
        {
            var apprenticeship = await DbContext.Apprenticeships.FirstOrDefaultAsync(x =>
                x.Key == apprenticeshipKey);
            return apprenticeship == null ? null :  new ApprenticeshipPrice
            {
                TotalPrice = apprenticeship.TotalPrice,
                AssessmentPrice = apprenticeship.EndPointAssessmentPrice,
                TrainingPrice = apprenticeship.TrainingPrice,
                FundingBandMaximum = apprenticeship.FundingBandMaximum
            };
        }

        public async Task<IEnumerable<DataTransferObjects.ApprenticeshipPrice>> GetPriceHistory(Guid apprenticeshipKey)
        {
            var dataModels = await DbContext.PriceHistories
                .Where(x => x.Key == apprenticeshipKey)
                .ToListAsync();

            return dataModels.Select(x => new ApprenticeshipPrice
            {
                TrainingPrice = x.TrainingPrice,
                AssessmentPrice = x.AssessmentPrice,
                TotalPrice = x.TotalPrice
            });
        }

        public async Task<Guid?> GetKey(string apprenticeshipHashedId)
        {
            var apprenticeship = await DbContext.Apprenticeships.FirstOrDefaultAsync(x =>
                x.ApprenticeshipHashedId == apprenticeshipHashedId);
            return apprenticeship?.Key;
        }
    }
}