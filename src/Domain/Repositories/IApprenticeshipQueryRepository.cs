using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Enums;

namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public interface IApprenticeshipQueryRepository
{
    Task<IEnumerable<DataTransferObjects.Apprenticeship>> GetAll(long ukprn, FundingPlatform? fundingPlatform);
    Task<DataTransferObjects.ApprenticeshipPrice?> GetPrice(Guid apprenticeshipKey);
    Task<DataTransferObjects.ApprenticeshipStartDate?> GetStartDate(Guid apprenticeshipKey);
    Task<IEnumerable<DataTransferObjects.ApprenticeshipPrice>> GetPriceHistory(Guid apprenticeshipKey);
    Task<PendingPriceChange?> GetPendingPriceChange(Guid apprenticeshipKey);
    Task<Guid?> GetKey(string apprenticeshipHashedId);
    Task<Guid?> GetKeyByApprenticeshipId(long apprenticeshipId);
}