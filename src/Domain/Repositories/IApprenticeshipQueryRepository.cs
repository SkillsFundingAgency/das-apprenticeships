﻿using SFA.DAS.Apprenticeships.DataTransferObjects;
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
    Task<List<ApprenticeshipWithEpisodes>?> GetApprenticeshipsWithEpisodes(long ukprn);
    Task<CurrentPartyIds?> GetCurrentPartyIds(Guid apprenticeshipKey);
    Task<LearnerStatusDetails?> GetLearnerStatus(Guid apprenticeshipKey);
}