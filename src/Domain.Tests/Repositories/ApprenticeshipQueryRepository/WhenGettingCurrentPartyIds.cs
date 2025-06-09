using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.TestHelpers;
using SFA.DAS.Learning.DataAccess;
using SFA.DAS.Learning.Domain.UnitTests.Helpers;

namespace SFA.DAS.Learning.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository;

public class WhenGettingCurrentPartyIds
{
    private Learning.Domain.Repositories.LearningQueryRepository _sut;
    private Fixture _fixture;
    private ApprenticeshipsDataContext _dbContext;

    [SetUp]
    public void Arrange()
    {
        _fixture = new Fixture();
    }

    [TearDown]
    public void CleanUp()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task ThenReturnNullWhenNoApprenticeshipFoundWithApprenticeshipKey()
    {
        // Arrange
        SetUpApprenticeshipQueryRepository();

        // Act
        var result = await _sut.GetCurrentPartyIds(_fixture.Create<Guid>());

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ThenTheCorrectCurrentPartyIdsAreReturned()
    {
        // Arrange
        SetUpApprenticeshipQueryRepository();
        var apprenticeshipKey = _fixture.Create<Guid>();

        var apprenticeship = await _dbContext.AddApprenticeship(apprenticeshipKey, false);
        var episode = apprenticeship.Episodes.Single();

        // Act
        var result = await _sut.GetCurrentPartyIds(apprenticeshipKey);

        // Assert
        result.Should().NotBeNull();
        result.Ukprn.Should().Be(episode.Ukprn);
        result.EmployerAccountId.Should().Be(episode.EmployerAccountId);
        result.ApprovalsApprenticeshipId.Should().Be(apprenticeship.ApprovalsApprenticeshipId);
    }

    private void SetUpApprenticeshipQueryRepository()
    {
        _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
        var logger = Mock.Of<ILogger<Learning.Domain.Repositories.LearningQueryRepository>>();
        _sut = new Learning.Domain.Repositories.LearningQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
    }
}