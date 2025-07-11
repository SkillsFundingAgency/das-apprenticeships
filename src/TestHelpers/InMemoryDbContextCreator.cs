using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.Learning.DataAccess;
using SFA.DAS.Learning.DataAccess.Entities.Learning;

namespace SFA.DAS.Learning.TestHelpers;

public static class InMemoryDbContextCreator
{
    public static LearningDataContext SetUpInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LearningDataContext>()
            .UseInMemoryDatabase("ApprenticeshipsDbContext" + Guid.NewGuid()).Options;

        var accountIdAuthorizer = new Mock<IAccountIdAuthorizer>();
        accountIdAuthorizer
            .Setup(x => x.ApplyAuthorizationFilterOnQueries(It.IsAny<DbSet<DataAccess.Entities.Learning.Learning>>()))
            .Returns<DbSet<DataAccess.Entities.Learning.Learning>>(x => x);

        return new LearningDataContext(options, accountIdAuthorizer.Object);
    }
}