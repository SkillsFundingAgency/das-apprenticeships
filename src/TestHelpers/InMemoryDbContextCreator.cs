using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.Apprenticeships.DataAccess;

namespace SFA.DAS.Apprenticeships.TestHelpers
{
    public static class InMemoryDbContextCreator
    {
        public static ApprenticeshipsDataContext SetUpInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>()
                .UseInMemoryDatabase("ApprenticeshipsDbContext" + Guid.NewGuid()).Options;
            var accountIdAuthorizer = Mock.Of<IAccountIdAuthorizer>();
            return new ApprenticeshipsDataContext(options, accountIdAuthorizer);
        }
    }
}