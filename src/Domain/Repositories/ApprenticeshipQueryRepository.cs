using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;

 namespace SFA.DAS.Apprenticeships.Domain.Repositories; 

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
         var apprenticeships = await DbContext.Apprenticeships
             .Include(x => x.Episodes)
             .Where(x => x.Episodes.Any(y => y.Ukprn == ukprn && (fundingPlatform == null || y.FundingPlatform == fundingPlatform)))
             .ToListAsync();
        
         var result = apprenticeships.Select(x => new DataTransferObjects.Apprenticeship { Uln = x.Uln, LastName = x.LastName, FirstName = x.FirstName });
         return result;
     }
     public async Task<Guid?> GetKeyByApprenticeshipId(long apprenticeshipId)
     {
         var apprenticeshipWithMatchingId = await DbContext.Apprenticeships
             .SingleOrDefaultAsync(x => x.ApprovalsApprenticeshipId == apprenticeshipId);
         return apprenticeshipWithMatchingId?.Key;
     }

     public async Task<ApprenticeshipPrice?> GetPrice(Guid apprenticeshipKey)
     {
         var apprenticeship = await DbContext.Apprenticeships
             .Include(x => x.Episodes)
             .FirstOrDefaultAsync(x => x.Key == apprenticeshipKey);

         var episodes = apprenticeship?.Episodes.ToList(); 
         var prices = episodes?.SelectMany(x => x.Prices).Where(x => !x.IsDeleted).ToList();

         var latestPrice = prices?.MaxBy(x => x.StartDate);
         if (latestPrice == null)
         {
             return null;
         }

         var latestEpisode = episodes?.MaxBy(x => x.Prices.Where(x => !x.IsDeleted).Select(x => x.StartDate));
         if (latestEpisode == null)
         {
             return null;
         }

         var firstPrice = prices?.MinBy(x => x.StartDate);
         if (firstPrice == null)
         {
             return null;
         }
         
         return new ApprenticeshipPrice
         {
             TotalPrice = latestPrice.TotalPrice,
             AssessmentPrice = latestPrice.EndPointAssessmentPrice,
             TrainingPrice = latestPrice.TrainingPrice,
             FundingBandMaximum = latestPrice.FundingBandMaximum,
             ApprenticeshipActualStartDate = firstPrice.StartDate,
             ApprenticeshipPlannedEndDate = latestPrice.EndDate,
             AccountLegalEntityId = latestEpisode.AccountLegalEntityId,
             UKPRN = latestEpisode.Ukprn
         };
     }
     public async Task<ApprenticeshipStartDate?> GetStartDate(Guid apprenticeshipKey)
     {
         var apprenticeship = await DbContext.Apprenticeships
             .Include(x => x.Episodes)
             .FirstOrDefaultAsync(x => x.Key == apprenticeshipKey);

         var episodes = apprenticeship?.Episodes.ToList(); 
         var prices = episodes?.SelectMany(x => x.Prices).Where(x => !x.IsDeleted).ToList();

         var latestPrice = prices?.MaxBy(x => x.StartDate);
         if (latestPrice == null)
         {
             return null;
         }

         var latestEpisode = episodes?.MaxBy(x => x.Prices.Where(x => !x.IsDeleted).Select(x => x.StartDate));
         if (latestEpisode == null)
         {
             return null;
         }

         var firstPrice = prices?.MinBy(x => x.StartDate);
         if (firstPrice == null)
         {
             return null;
         }

         return apprenticeship == null ? null : new ApprenticeshipStartDate
         {
	         ApprenticeshipKey = apprenticeship.Key,
	         ActualStartDate = firstPrice.StartDate,
	         PlannedEndDate = latestPrice.EndDate,
	         AccountLegalEntityId = latestEpisode.AccountLegalEntityId,
	         UKPRN = latestEpisode.Ukprn,
	         ApprenticeDateOfBirth = apprenticeship.DateOfBirth,
	         CourseCode = latestEpisode.TrainingCode,
	         CourseVersion = latestEpisode.TrainingCourseVersion
         };
     }

     public async Task<PendingPriceChange?> GetPendingPriceChange(Guid apprenticeshipKey)
     {
         _logger.LogInformation("Getting pending price change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);

         PendingPriceChange? pendingPriceChange = null;

         try
         {
             var apprenticeship = await DbContext.Apprenticeships
                 .Include(x => x.PriceHistories)
                 .Where(x => x.Key == apprenticeshipKey && x.PriceHistories.Any(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created))
                 .SingleOrDefaultAsync();

             var episodes = apprenticeship?.Episodes.ToList(); 
             var prices = episodes?.SelectMany(x => x.Prices).Where(x => !x.IsDeleted).ToList();

             var latestPrice = prices?.MaxBy(x => x.StartDate);
             if (latestPrice == null)
             {
                 return null;
             }

             var latestEpisode = episodes?.MaxBy(x => x.Prices.Where(x => !x.IsDeleted).Select(x => x.StartDate));
             if (latestEpisode == null)
             {
                 return null;
             }

             var priceHistory = apprenticeship!.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created);

             return new PendingPriceChange
             {
                 FirstName = apprenticeship!.FirstName,
                 LastName = apprenticeship.LastName,
                 OriginalTrainingPrice = latestPrice.TrainingPrice,
                 OriginalAssessmentPrice = latestPrice.EndPointAssessmentPrice,
                 OriginalTotalPrice = latestPrice.TotalPrice,
                 PendingTrainingPrice = priceHistory.TrainingPrice,
                 PendingAssessmentPrice = priceHistory.AssessmentPrice,
                 PendingTotalPrice = priceHistory.TotalPrice,
                 EffectiveFrom = priceHistory.EffectiveFromDate,
                 Reason = priceHistory.ChangeReason,
                 Ukprn = latestEpisode.Ukprn,
                 ProviderApprovedDate = priceHistory.ProviderApprovedDate,
                 EmployerApprovedDate = priceHistory.EmployerApprovedDate,
                 AccountLegalEntityId = latestEpisode.AccountLegalEntityId,
                 Initiator = priceHistory.Initiator.ToString()
             };
         }
         catch(Exception e)
         {
             _logger.LogError(e, "Error getting pending price change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);
         }

         return pendingPriceChange;
     }

     public async Task<Guid?> GetKey(string apprenticeshipHashedId)
     {
         var apprenticeship = await DbContext.Apprenticeships.FirstOrDefaultAsync(x =>
             x.ApprenticeshipHashedId == apprenticeshipHashedId);
         return apprenticeship?.Key;
     }

     public async Task<PendingStartDateChange?> GetPendingStartDateChange(Guid apprenticeshipKey)
     {
         _logger.LogInformation("Getting pending start date change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);

         PendingStartDateChange? pendingStartDateChange = null;

         try
         {
             var apprenticeship = await DbContext.Apprenticeships
                 .Include(x => x.StartDateChanges)
                 .Where(x => x.Key == apprenticeshipKey && x.StartDateChanges.Any(y => y.RequestStatus == ChangeRequestStatus.Created))
                 .SingleOrDefaultAsync();
             var episodes = apprenticeship?.Episodes.ToList(); 
             var prices = episodes?.SelectMany(x => x.Prices).Where(x => !x.IsDeleted).ToList();

             var latestPrice = prices?.MaxBy(x => x.StartDate);
             if (latestPrice == null)
             {
                 return null;
             }

             var latestEpisode = episodes?.MaxBy(x => x.Prices.Where(x => !x.IsDeleted).Select(x => x.StartDate));
             if (latestEpisode == null)
             {
                 return null;
             }

             var firstPrice = prices?.MinBy(x => x.StartDate);
             if (firstPrice == null)
             {
                 return null;
             }

             var startDateChange = apprenticeship!.StartDateChanges.Single(y => y.RequestStatus == ChangeRequestStatus.Created);

             return new PendingStartDateChange
             {
                 Reason = startDateChange.Reason,
                 Ukprn = latestEpisode.Ukprn,
                 ProviderApprovedDate = startDateChange.ProviderApprovedDate,
                 EmployerApprovedDate = startDateChange.EmployerApprovedDate,
                 AccountLegalEntityId = latestEpisode.AccountLegalEntityId,
                 Initiator = startDateChange.Initiator.ToString(),
                 OriginalActualStartDate = firstPrice.StartDate,
                 PendingActualStartDate = startDateChange.ActualStartDate,
                 OriginalPlannedEndDate = latestPrice.EndDate,
                 PendingPlannedEndDate = startDateChange.PlannedEndDate
             };
         }
         catch (Exception e)
         {
             _logger.LogError(e, "Error getting pending start date change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);
         }

         return pendingStartDateChange;
     }

     public async Task<PaymentStatus?> GetPaymentStatus(Guid apprenticeshipKey)
     {
        PaymentStatus? paymentStatus = null;

	     try
	     { 
             var apprenticeship = await DbContext.Apprenticeships
                 .Include(x => x.Episodes)
                 .Include(x => x.FreezeRequests)
                 .FirstOrDefaultAsync(x => x.Key == apprenticeshipKey);

             var episodes = apprenticeship?.Episodes.ToList(); 
             var latestEpisode = episodes?.MaxBy(x => x.Prices.Where(x => !x.IsDeleted).Select(x => x.StartDate));
             if (latestEpisode == null)
             {
                 return null;
             }

             paymentStatus = new PaymentStatus() { IsFrozen = latestEpisode.PaymentsFrozen };
             
             if (paymentStatus.IsFrozen)
             {
                 var activeFreezeRequest = apprenticeship!.FreezeRequests.Single(x => x.ApprenticeshipKey == apprenticeshipKey && !x.Unfrozen);
                 paymentStatus.Reason = activeFreezeRequest.Reason;
                 paymentStatus.FrozenOn = activeFreezeRequest.FrozenDateTime;
             }
	     }
	     catch (Exception e)
	     {
		     _logger.LogError(e, "Error getting payment status for apprenticeship {apprenticeshipKey}", apprenticeshipKey);
		 }

	     return paymentStatus;
     }
 }