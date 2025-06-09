using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Enums;

namespace SFA.DAS.Learning.Domain.Repositories;

public interface IApprenticeshipQueryRepository
{
    Task<IEnumerable<Learning.DataTransferObjects.Apprenticeship>> GetAll(long ukprn, FundingPlatform? fundingPlatform);
    Task<PagedResult<Learning.DataTransferObjects.Apprenticeship>> GetByDates(long ukprn, DateRange dates, int limit, int offset, CancellationToken cancellationToken);
    Task<Guid?> GetKeyByApprenticeshipId(long apprenticeshipId);
    Task<ApprenticeshipPrice?> GetPrice(Guid apprenticeshipKey);
    Task<ApprenticeshipStartDate?> GetStartDate(Guid apprenticeshipKey);
    Task<PendingPriceChange?> GetPendingPriceChange(Guid apprenticeshipKey);
    Task<Guid?> GetKey(string apprenticeshipHashedId);
    Task<PendingStartDateChange?> GetPendingStartDateChange(Guid apprenticeshipKey);
    Task<PaymentStatus?> GetPaymentStatus(Guid apprenticeshipKey);

    /// <summary>
    /// Get apprenticeships with episodes for a provider
    /// </summary>
    /// <param name="ukprn">The unique provider reference number. Only apprenticeships where the episode with this provider reference will be returned.</param>
    /// <param name="activeOnDate">If populated, will return only apprenticeships that are active on this date</param>
    Task<List<ApprenticeshipWithEpisodes>?> GetApprenticeshipsWithEpisodes(long ukprn, DateTime? activeOnDate = null);
    Task<CurrentPartyIds?> GetCurrentPartyIds(Guid apprenticeshipKey);
    Task<LearnerStatusDetails?> GetLearnerStatus(Guid apprenticeshipKey);
}