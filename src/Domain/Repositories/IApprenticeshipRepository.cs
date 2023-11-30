using SFA.DAS.Apprenticeships.Domain.Apprenticeship;

namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public interface IApprenticeshipRepository
{
    Task Add(ApprenticeshipDomainModel apprenticeship);
    Task<ApprenticeshipDomainModel> Get(Guid key);
    Task Update(ApprenticeshipDomainModel apprenticeship);
}