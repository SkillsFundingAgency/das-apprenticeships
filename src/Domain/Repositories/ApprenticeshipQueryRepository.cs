using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.DataAccess.Extensions;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.Validators;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public class ApprenticeshipQueryRepository(Lazy<ApprenticeshipsDataContext> dbContext, ILogger<ApprenticeshipQueryRepository> logger)
    : IApprenticeshipQueryRepository
{
    private ApprenticeshipsDataContext DbContext => dbContext.Value;

    public async Task<IEnumerable<DataTransferObjects.Apprenticeship>> GetAll(long ukprn, FundingPlatform? fundingPlatform)
    {
        var apprenticeships = await DbContext.Apprenticeships
            .Include(x => x.Episodes)
            .Where(x => x.Episodes.Any(y => y.Ukprn == ukprn && (fundingPlatform == null || y.FundingPlatform == fundingPlatform)))
            .ToListAsync();

        var result = apprenticeships.Select(x => new DataTransferObjects.Apprenticeship { Uln = x.Uln, LastName = x.LastName, FirstName = x.FirstName });
        return result;
    }

    public async Task<PagedResult<DataTransferObjects.Apprenticeship>> GetByDates(long ukprn, DateRange dates, int limit, int offset, CancellationToken cancellationToken)
    {
        var query = DbContext.ApprenticeshipsDbSet
            .Include(x => x.Episodes)
            .ThenInclude(x => x.Prices.Where(y => !y.IsDeleted))
            .Where(x => x.Episodes.Any(e => e.Ukprn == ukprn))
            .Where(x => x.Episodes.Any(e => e.Prices.Any(p => dates.Start >= p.StartDate & dates.End <= p.EndDate)))
            .Where(x => x.Episodes.Any(e => e.LearningStatus == nameof(LearnerStatus.Active)))
            .OrderBy(x => x.ApprovalsApprenticeshipId)
            .AsNoTracking();

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalItems / limit);

        var result = await query
            .Skip(offset)
            .Take(limit)
            .Select(x => new DataTransferObjects.Apprenticeship
            {
                Uln = x.Uln,
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<DataTransferObjects.Apprenticeship>
        {
            Data = result,
            TotalItems = totalItems,
            TotalPages = totalPages,
        };
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
            .ThenInclude(x => x.Prices)
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
            .ThenInclude(x => x.Prices)
            .FirstOrDefaultAsync(x => x.Key == apprenticeshipKey);

        var episodes = apprenticeship?.Episodes.ToList();
        var prices = episodes?.SelectMany(x => x.Prices).Where(x => !x.IsDeleted).ToList();

        var latestPrice = prices?.MaxBy(x => x.StartDate);
        if (latestPrice == null)
        {
            return null;
        }

        var latestEpisode = episodes?.MaxBy(x => x.Prices.Where(y => !y.IsDeleted).Max(y => y.StartDate));
        if (latestEpisode == null)
        {
            return null;
        }

        var firstPrice = prices?.MinBy(x => x.StartDate);
        if (firstPrice == null)
        {
            return null;
        }

        return apprenticeship == null
            ? null
            : new ApprenticeshipStartDate
            {
                ApprenticeshipKey = apprenticeship.Key,
                ActualStartDate = firstPrice.StartDate,
                PlannedEndDate = latestPrice.EndDate,
                AccountLegalEntityId = latestEpisode.AccountLegalEntityId,
                UKPRN = latestEpisode.Ukprn,
                ApprenticeDateOfBirth = apprenticeship.DateOfBirth,
                CourseCode = latestEpisode.TrainingCode,
                CourseVersion = latestEpisode.TrainingCourseVersion,
                SimplifiedPaymentsMinimumStartDate = Constants.SimplifiedPaymentsMinimumStartDate
            };
    }

    public async Task<PendingPriceChange?> GetPendingPriceChange(Guid apprenticeshipKey)
    {
        logger.LogInformation("Getting pending price change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);

        PendingPriceChange? pendingPriceChange = null;

        try
        {
            var apprenticeship = await DbContext.Apprenticeships
                .Include(x => x.PriceHistories)
                .Include(x => x.Episodes).ThenInclude(x => x.Prices)
                .Where(x => x.Key == apprenticeshipKey && x.PriceHistories.Any(y => y.PriceChangeRequestStatus == ChangeRequestStatus.Created))
                .SingleOrDefaultAsync();

            var episodes = apprenticeship?.Episodes.ToList();
            var prices = episodes?.SelectMany(x => x.Prices).Where(x => !x.IsDeleted).ToList();

            var latestPrice = prices?.MaxBy(x => x.StartDate);
            if (latestPrice == null)
            {
                return null;
            }

            var latestEpisode = episodes?.MaxBy(x => x.Prices.Where(y => !y.IsDeleted).Max(y => y.StartDate));
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
        catch (Exception e)
        {
            logger.LogError(e, "Error getting pending price change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);
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
        logger.LogInformation("Getting pending start date change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);

        PendingStartDateChange? pendingStartDateChange = null;

        try
        {
            var apprenticeship = await DbContext.Apprenticeships
                .Include(x => x.StartDateChanges)
                .Include(x => x.Episodes).ThenInclude(x => x.Prices)
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
            logger.LogError(e, "Error getting pending start date change for apprenticeship {apprenticeshipKey}", apprenticeshipKey);
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
            logger.LogError(e, "Error getting payment status for apprenticeship {apprenticeshipKey}", apprenticeshipKey);
        }

        return paymentStatus;
    }

    public async Task<List<ApprenticeshipWithEpisodes>?> GetApprenticeshipsWithEpisodes(long ukprn)
    {
        List<ApprenticeshipWithEpisodes>? apprenticeshipWithEpisodes = null;
        try
        {
            var withdrawFromStartReason = WithdrawReason.WithdrawFromStart.ToString();
            var withdrawFromPrivateBeta = WithdrawReason.WithdrawFromBeta.ToString();

            var apprenticeships = await DbContext.Apprenticeships
                .Include(x => x.Episodes)
                .ThenInclude(x => x.Prices.Where(y => !y.IsDeleted))
                .Include(x => x.WithdrawalRequests)
                .Where(x => x.WithdrawalRequests == null || !x.WithdrawalRequests.Any(y => y.Reason == withdrawFromStartReason || y.Reason == withdrawFromPrivateBeta))
                .Where(x => x.Episodes.Any(e => e.Ukprn == ukprn))
                .ToListAsync();

            apprenticeshipWithEpisodes = apprenticeships.Select(apprenticeship =>
                new ApprenticeshipWithEpisodes(
                    apprenticeship.Key,
                    apprenticeship.Uln,
                    apprenticeship.GetStartDate(),
                    apprenticeship.GetPlannedEndDate(),
                    apprenticeship.Episodes.Select(ep =>
                            new Episode(ep.Key, ep.TrainingCode, ep.Prices.Select(p =>
                                new EpisodePrice(p.Key, p.StartDate, p.EndDate, p.TrainingPrice, p.EndPointAssessmentPrice, p.TotalPrice, p.FundingBandMaximum)).ToList()))
                        .ToList(),
                    apprenticeship.GetAgeAtStartOfApprenticeship(),
                    apprenticeship.GetWithdrawnDate())
            ).ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting apprenticeships with episodes for provider UKPRN {Ukprn}", ukprn);
        }

        return apprenticeshipWithEpisodes;
    }

    public async Task<CurrentPartyIds?> GetCurrentPartyIds(Guid apprenticeshipKey)
    {
        CurrentPartyIds? currentPartyIds = null;

        try
        {
            var apprenticeship = await DbContext.Apprenticeships
                .Where(a => a.Key == apprenticeshipKey)
                .Include(a => a.Episodes)
                .SingleOrDefaultAsync();

            if (apprenticeship == null)
                return null;

            var episode = apprenticeship.GetEpisode();
            currentPartyIds = new CurrentPartyIds(episode.Ukprn, episode.EmployerAccountId, apprenticeship.ApprovalsApprenticeshipId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting current party ids for apprenticeship key {key}", apprenticeshipKey);
        }

        return currentPartyIds;
    }

    public async Task<LearnerStatusDetails?> GetLearnerStatus(Guid apprenticeshipKey)
    {
        LearnerStatusDetails? learnerStatus = null;

        try
        {
            var apprenticeship = await DbContext.Apprenticeships
                .Where(a => a.Key == apprenticeshipKey)
                .Include(a => a.Episodes)
                .Include(a => a.WithdrawalRequests)
                .SingleOrDefaultAsync();

            if (apprenticeship == null)
            {
                logger.LogInformation("Apprenticeship not found for apprenticeship key {key} when attempting to get learner status", apprenticeshipKey);
                return null;
            }


            var episode = apprenticeship.GetEpisode();

            if (Enum.TryParse<LearnerStatus>(episode.LearningStatus, out var parsedStatus))
            {
                var withdrawalRequest = apprenticeship.WithdrawalRequests.SingleOrDefault(x => x.EpisodeKey == episode.Key);
                learnerStatus = new LearnerStatusDetails
                {
                    LearnerStatus = parsedStatus,
                    WithdrawalChangedDate = withdrawalRequest?.CreatedDate,
                    WithdrawalReason = withdrawalRequest?.Reason,
                    LastDayOfLearning = withdrawalRequest?.LastDayOfLearning
                };
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting learner status for apprenticeship key {key}", apprenticeshipKey);
        }

        return learnerStatus;
    }
}