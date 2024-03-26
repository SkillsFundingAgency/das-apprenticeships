namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public interface IApprovalQueryRepository
{
    Task<Guid?> GetKeyByApprenticeshipId(long apprenticeshipId);
}