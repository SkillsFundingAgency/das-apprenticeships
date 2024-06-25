using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.Domain.Repositories;

//TODO RENAME MOVE TO MAIN QUERY REPOSITORY
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
            .SingleOrDefaultAsync(x => x.ApprovalsApprenticeshipId == apprenticeshipId);
        return apprenticeshipWithMatchingId?.Key;
    }
}