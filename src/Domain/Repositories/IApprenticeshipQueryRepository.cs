using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;
using SFA.DAS.Apprenticeships.InnerApi.Responses;

namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public interface IApprenticeshipQueryRepository
{
    Task<IEnumerable<DataTransferObjects.Apprenticeship>> GetAll(long ukprn, FundingPlatform? fundingPlatform);
    Task<PagedResult<DataTransferObjects.Apprenticeship>> GetByDates(long ukprn, DateRange dates,int page, int? pageSize, int limit, int offset, CancellationToken cancellationToken);
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