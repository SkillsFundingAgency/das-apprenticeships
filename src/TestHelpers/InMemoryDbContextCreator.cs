using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.Learning.DataAccess;
using SFA.DAS.Learning.DataAccess.Entities.Apprenticeship;

namespace SFA.DAS.Learning.TestHelpers;

public static class InMemoryDbContextCreator
{
    public static ApprenticeshipsDataContext SetUpInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApprenticeshipsDataContext>()
            .UseInMemoryDatabase("ApprenticeshipsDbContext" + Guid.NewGuid()).Options;

        var accountIdAuthorizer = new Mock<IAccountIdAuthorizer>();
        accountIdAuthorizer
            .Setup(x => x.ApplyAuthorizationFilterOnQueries(It.IsAny<DbSet<Apprenticeship>>()))
            .Returns<DbSet<Apprenticeship>>(x => x);

        return new ApprenticeshipsDataContext(options, accountIdAuthorizer.Object);
    }
}