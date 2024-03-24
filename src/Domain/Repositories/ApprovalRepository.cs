﻿using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.Domain.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly Lazy<ApprenticeshipsDataContext> _lazyContext;
        private ApprenticeshipsDataContext DbContext => _lazyContext.Value;

        public ApprovalRepository(Lazy<ApprenticeshipsDataContext> dbContext)
        {
            _lazyContext = dbContext;
        }

        public async Task<Guid?> GetKeyByApprenticeshipId(long apprenticeshipId)
        {
            var approval = await DbContext.Approvals.FirstOrDefaultAsync(x =>
                x.ApprovalsApprenticeshipId == apprenticeshipId);
            return approval?.ApprenticeshipKey;
        }
    }
}