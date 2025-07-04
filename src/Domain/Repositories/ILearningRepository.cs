using SFA.DAS.Learning.Domain.Apprenticeship;

namespace SFA.DAS.Learning.Domain.Repositories;

public interface ILearningRepository
{
    Task Add(LearningDomainModel learning);
    Task<LearningDomainModel> Get(Guid key);
    Task<LearningDomainModel?> GetByUln(string uln);
    Task<LearningDomainModel?> Get(string uln, long approvalsApprenticeshipId);
    Task Update(LearningDomainModel learning);
}