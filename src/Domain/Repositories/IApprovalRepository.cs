namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public interface IApprovalRepository
{
    Task<Guid?> GetKeyByApprenticeshipId(long apprenticeshipId);
}