using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Repositories
{
    public class ApprenticeshipQueryRepository : IApprenticeshipQueryRepository
    {
        private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
        private readonly ILogger<ApprenticeshipQueryRepository> _logger;
        private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

        public ApprenticeshipQueryRepository(Lazy<ApprenticeshipsDataContext> dbContext, ILogger<ApprenticeshipQueryRepository> logger)
        {
            _lazyContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<DataTransferObjects.Apprenticeship>> GetAll(long ukprn, FundingPlatform? fundingPlatform)
        {
            var dataModels = await DbContext.Apprenticeships
                .Where(x => x.Ukprn == ukprn)
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
                AccountLegalEntityId = apprenticeship.AccountLegalEntityId,
                UKPRN = apprenticeship.Ukprn
            };
        }
        public async Task<ApprenticeshipStartDate?> GetStartDate(Guid apprenticeshipKey)
        {
            var apprenticeship = await DbContext.Apprenticeships.FirstOrDefaultAsync(x =>
                x.Key == apprenticeshipKey);

            return apprenticeship == null ? null : new ApprenticeshipStartDate
            {
                ApprenticeshipKey = apprenticeship.Key,
                ActualStartDate = apprenticeship.ActualStartDate,
                PlannedEndDate = apprenticeship.PlannedEndDate,
                AccountLegalEntityId = apprenticeship.AccountLegalEntityId,
                UKPRN = apprenticeship.Ukprn
            };
        }

        public async Task<PendingPriceChange?> GetPendingPriceChange(Guid apprenticeshipKey)
        {
            _logger.LogInformation("Getting pending price change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);

            PendingPriceChange? pendingPriceChange = null;

            try
            {
                pendingPriceChange = await DbContext.Apprenticeships
                .Include(x => x.PriceHistories)
                .Where(x => x.Key == apprenticeshipKey && x.PriceHistories.Any(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created))
                .Select(PriceHistoryToPendingPriceChange())
                .SingleOrDefaultAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error getting pending price change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);
            }

            return pendingPriceChange;
        }

        private static Expression<Func<DataAccess.Entities.Apprenticeship.Apprenticeship, PendingPriceChange>> PriceHistoryToPendingPriceChange()
        {
	        return x => new PendingPriceChange
	        {
                FirstName = x.FirstName,
                LastName = x.LastName,
		        OriginalTrainingPrice = x.TrainingPrice,
                OriginalAssessmentPrice = x.EndPointAssessmentPrice,
                OriginalTotalPrice = x.TotalPrice,
                PendingTrainingPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).TrainingPrice,
                PendingAssessmentPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).AssessmentPrice,
                PendingTotalPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).TotalPrice,
                EffectiveFrom = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).EffectiveFromDate,
                Reason = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).ChangeReason,
                Ukprn = x.Ukprn,
                ProviderApprovedDate = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).ProviderApprovedDate,
                EmployerApprovedDate = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).EmployerApprovedDate,
                AccountLegalEntityId = x.AccountLegalEntityId,
                Initiator = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == PriceChangeRequestStatus.Created).Initiator.ToString()
            };
        }

        public async Task<Guid?> GetKey(string apprenticeshipHashedId)
        {
            var apprenticeship = await DbContext.Apprenticeships.FirstOrDefaultAsync(x =>
                x.ApprenticeshipHashedId == apprenticeshipHashedId);
            return apprenticeship?.Key;
        }
	}
}