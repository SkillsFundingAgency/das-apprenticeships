using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.DataAccess;
using SFA.DAS.Apprenticeships.Domain.Apprenticeship;
using SFA.DAS.Apprenticeships.Domain.UnitTests.Helpers;
using SFA.DAS.Apprenticeships.TestHelpers;

namespace SFA.DAS.Apprenticeships.Domain.UnitTests.Repositories.ApprenticeshipQueryRepository;

public class WhenGettingLearnerStatus
{
    private Domain.Repositories.ApprenticeshipQueryRepository _sut;
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
    public async Task ThenReturnNullWhenNoApprenticeshipFoundWithKey()
    {
        // Arrange
        SetUpApprenticeshipQueryRepository();

        // Act
        var result = await _sut.GetLearnerStatus(_fixture.Create<Guid>());

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ThenTheCorrectLearnerStatusIsReturned()
    {
        // Arrange
        SetUpApprenticeshipQueryRepository();
        var apprenticeshipKey = _fixture.Create<Guid>();

        var apprenticeship = await _dbContext.AddApprenticeship(apprenticeshipKey, false, learnerStatus: LearnerStatus.Active);

        // Act
        var result = await _sut.GetLearnerStatus(apprenticeshipKey);

        // Assert
        result.Should().NotBeNull();
        result?.LearnerStatus.Should().Be(Enum.Parse<LearnerStatus>(apprenticeship.Episodes.First().LearningStatus));
    }

    [Test]
    public async Task ThenTheCorrectLearnerStatusIsReturnedWhenWithdrawn()
    {
        // Arrange
        SetUpApprenticeshipQueryRepository();
        var apprenticeshipKey = _fixture.Create<Guid>();

        var apprenticeship = await _dbContext.AddApprenticeship(apprenticeshipKey, false, learnerStatus: LearnerStatus.Withdrawn, addWithdrawalRequest: true);

        // Act
        var result = await _sut.GetLearnerStatus(apprenticeshipKey);

        // Assert
        result.Should().NotBeNull();
        result?.LearnerStatus.Should().Be(Enum.Parse<LearnerStatus>(apprenticeship.Episodes.First().LearningStatus));
        result?.WithdrawalChangedDate.Should().Be(apprenticeship.WithdrawalRequests.First().CreatedDate);
        result?.WithdrawalReason.Should().Be(apprenticeship.WithdrawalRequests.First().Reason);
        result?.LastDayOfLearning.Should().Be(apprenticeship.WithdrawalRequests.First().LastDayOfLearning);
    }

    private void SetUpApprenticeshipQueryRepository()
    {
        _dbContext = InMemoryDbContextCreator.SetUpInMemoryDbContext();
        var logger = Mock.Of<ILogger<Domain.Repositories.ApprenticeshipQueryRepository>>();
        _sut = new Domain.Repositories.ApprenticeshipQueryRepository(new Lazy<ApprenticeshipsDataContext>(_dbContext), logger);
    }
}