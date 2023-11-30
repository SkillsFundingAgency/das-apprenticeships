namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public interface IApprenticeshipRepository
{
    Task Add(DataAccess.Entities.Apprenticeship.Apprenticeship apprenticeship);
    Task<DataAccess.Entities.Apprenticeship.Apprenticeship> Get(Guid key);
    Task Update(DataAccess.Entities.Apprenticeship.Apprenticeship apprenticeship);
}