using SFA.DAS.Learning.Domain.Apprenticeship;

namespace SFA.DAS.Learning.Domain.Repositories;

public interface ILearningRepository
{
    Task Add(ApprenticeshipDomainModel apprenticeship);
    Task<ApprenticeshipDomainModel> Get(Guid key);
    Task<ApprenticeshipDomainModel?> GetByUln(string uln);
    Task<ApprenticeshipDomainModel?> Get(string uln, long approvalsApprenticeshipId);
    Task Update(ApprenticeshipDomainModel apprenticeship);
}