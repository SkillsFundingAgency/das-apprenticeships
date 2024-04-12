using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.Domain.Repositories;

public class ApprovalQueryRepository : IApprovalQueryRepository
{
    private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
    private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

    public ApprovalQueryRepository(Lazy<ApprenticeshipsDataContext> dbContext)
    {
        _lazyContext = dbContext;
    }

    public async Task<Guid?> GetKeyByApprenticeshipId(long apprenticeshipId)
    {
        var apprenticeshipWithMatchingId = await DbContext.Apprenticeships
            .Include(x => x.Approvals)
            .SingleOrDefaultAsync(x => x.Approvals.Any(x => x.ApprovalsApprenticeshipId == apprenticeshipId));
        return apprenticeshipWithMatchingId?.Approvals.FirstOrDefault()?.ApprenticeshipKey;
    }
}