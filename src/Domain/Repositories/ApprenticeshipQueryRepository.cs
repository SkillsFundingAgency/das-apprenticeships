using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Repositories
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
                .Where(x => x.UKPRN == ukprn)
                .Include(x => x.Approvals)
                .Where(a => a.Approvals.Any(c => (fundingPlatform == null || c.FundingPlatform == fundingPlatform)))
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
                FundingBandMaximum = apprenticeship.FundingBandMaximum,
                ApprenticeshipActualStartDate = apprenticeship.ActualStartDate,
                ApprenticeshipPlannedEndDate = apprenticeship.PlannedEndDate,
                AccountLegalEntityId = apprenticeship.AccountLegalEntityId
            };
        }

        public async Task<PendingPriceChange?> GetPendingPriceChange(Guid apprenticeshipKey)
        {
            var pendingPriceChange = await DbContext.Apprenticeships
	            .Include(x => x.PriceHistories)
                .Where(x => x.Key == apprenticeshipKey && x.PriceHistories.Any(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created))
                .Select(PriceHistoryToPendingPriceChange())
                .SingleOrDefaultAsync();

            return pendingPriceChange;
        }

        private static Expression<Func<DataAccess.Entities.Apprenticeship.Apprenticeship, PendingPriceChange>> PriceHistoryToPendingPriceChange()
        {
	        return x => new PendingPriceChange
	        {
		        OriginalTrainingPrice = x.TrainingPrice,
                OriginalAssessmentPrice = x.EndPointAssessmentPrice,
                OriginalTotalPrice = x.TotalPrice,
                PendingTrainingPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).TrainingPrice,
                PendingAssessmentPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).AssessmentPrice,
                PendingTotalPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).TotalPrice,
                EffectiveFrom = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).EffectiveFromDate,
                //Reason = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).
			};
        }

		private static Expression<Func<PriceHistory, ApprenticeshipPrice>> PriceHistoryToApprenticeshipPrice()
        {
            return x => new ApprenticeshipPrice
            {
                TrainingPrice = x.TrainingPrice,
                AssessmentPrice = x.AssessmentPrice,
                TotalPrice = x.TotalPrice
            };
        }

        public async Task<Guid?> GetKey(string apprenticeshipHashedId)
        {
            var apprenticeship = await DbContext.Apprenticeships.FirstOrDefaultAsync(x =>
                x.ApprenticeshipHashedId == apprenticeshipHashedId);
            return apprenticeship?.Key;
        }

        public async Task<Guid?> GetKeyByApprenticeshipId(long apprenticeshipId)
        {
	        var approval = await DbContext.Approvals.FirstOrDefaultAsync(x =>
		        x.ApprovalsApprenticeshipId == apprenticeshipId);
	        return approval?.ApprenticeshipKey;
        }
	}
}