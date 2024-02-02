using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Infrastructure;

namespace SFA.DAS.Apprenticeships.TestHelpers
{
    public static class InMemoryDbContextCreator
    {
        public static ApprenticeshipsDataContext SetUpInMemoryDbContext(IAccountIdClaimsHandler accountIdClaimsHandler)
        {
            var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>()
                .UseInMemoryDatabase("ApprenticeshipsDbContext" + Guid.NewGuid()).Options;
            return new ApprenticeshipsDataContext(options, accountIdClaimsHandler);
        }
    }
}
