using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public interface IApprenticeshipRepository
{
    Task Add(ApprenticeshipDomainModel apprenticeship);
    Task<ApprenticeshipDomainModel> Get(Guid key);
    Task<ApprenticeshipDomainModel?> GetByUln(string uln);
    Task<ApprenticeshipDomainModel?> Get(string uln, long approvalsApprenticeshipId);
    Task Update(ApprenticeshipDomainModel apprenticeship);
}