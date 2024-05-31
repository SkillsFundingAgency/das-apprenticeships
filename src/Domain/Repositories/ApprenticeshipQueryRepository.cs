using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Entities.Apprenticeship;
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
             UKPRN = apprenticeship.Ukprn,
                ApprenticeDateOfBirth = apprenticeship.DateOfBirth,
                CourseCode = apprenticeship.TrainingCode,
                CourseVersion = apprenticeship.TrainingCourseVersion
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
                 .Where(x => x.Key == apprenticeshipKey && x.PriceHistories.Any(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created))
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
             PendingTrainingPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created).TrainingPrice,
             PendingAssessmentPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created).AssessmentPrice,
             PendingTotalPrice = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created).TotalPrice,
             EffectiveFrom = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created).EffectiveFromDate,
             Reason = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created).ChangeReason,
             Ukprn = x.Ukprn,
             ProviderApprovedDate = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created).ProviderApprovedDate,
             EmployerApprovedDate = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created).EmployerApprovedDate,
             AccountLegalEntityId = x.AccountLegalEntityId,
             Initiator = x.PriceHistories.Single(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created).Initiator.ToString()
         };
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
             pendingStartDateChange = await DbContext.Apprenticeships
                 .Include(x => x.StartDateChanges)
                 .Where(x => x.Key == apprenticeshipKey && x.StartDateChanges.Any(y => y.RequestStatus == ChangeRequestStatus.Created))
                 .Select(StartDateChangeToPendingStartDateChange())
                 .SingleOrDefaultAsync();
         }
         catch (Exception e)
         {
             _logger.LogError(e, "Error getting pending start date change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);
         }

         return pendingStartDateChange;
     }

     private static Expression<Func<DataAccess.Entities.Apprenticeship.Apprenticeship, PendingStartDateChange>> StartDateChangeToPendingStartDateChange()
     {
         return x => new PendingStartDateChange
         {
             Reason = x.StartDateChanges.Single(y => y.RequestStatus == ChangeRequestStatus.Created).Reason,
             Ukprn = x.Ukprn,
             ProviderApprovedDate = x.StartDateChanges.Single(y => y.RequestStatus == ChangeRequestStatus.Created).ProviderApprovedDate,
             EmployerApprovedDate = x.StartDateChanges.Single(y => y.RequestStatus == ChangeRequestStatus.Created).EmployerApprovedDate,
             AccountLegalEntityId = x.AccountLegalEntityId,
             Initiator = x.StartDateChanges.Single(y => y.RequestStatus == ChangeRequestStatus.Created).Initiator.ToString(),
             OriginalActualStartDate = x.ActualStartDate.GetValueOrDefault(),
             PendingActualStartDate = x.StartDateChanges.Single(y => y.RequestStatus == ChangeRequestStatus.Created).ActualStartDate,
			 OriginalPlannedEndDate = x.PlannedEndDate.GetValueOrDefault(),
			 PendingPlannedEndDate = x.StartDateChanges.Single(y => y.RequestStatus == ChangeRequestStatus.Created).PlannedEndDate
		 };
     }
 }