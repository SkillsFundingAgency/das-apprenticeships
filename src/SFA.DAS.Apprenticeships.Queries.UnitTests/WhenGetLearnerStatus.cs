using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.DataTransferObjects;
using SFA.DAS.Apprenticeships.Domain.Repositories;
using SFA.DAS.Apprenticeships.Infrastructure.Services;
using SFA.DAS.Apprenticeships.Queries.GetLearnerStatus;
using SFA.DAS.Apprenticeships.Types;

namespace SFA.DAS.Apprenticeships.Queries.UnitTests;


[TestFixture]
public class WhenGetLearnerStatus
{
    private Mock<IApprenticeshipQueryRepository> _apprenticeshipQueryRepositoryMock;
    private Mock<ISystemClockService> _systemClockServiceMock;
    private Mock<ILogger<GetLearnerStatusQueryHandler>> _loggerMock;
    private GetLearnerStatusQueryHandler _handler;
    private Fixture _fixture = new Fixture();

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipQueryRepositoryMock = new Mock<IApprenticeshipQueryRepository>();
        _systemClockServiceMock = new Mock<ISystemClockService>();
        _loggerMock = new Mock<ILogger<GetLearnerStatusQueryHandler>>();
        _handler = new GetLearnerStatusQueryHandler(
            _apprenticeshipQueryRepositoryMock.Object,
            _systemClockServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task Then_ShouldReturnNull_WhenStartDateIsNull()
    {
        // Arrange
        var query = _fixture.Create<GetLearnerStatusRequest>();
        MockStartDateInRepository(null);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task Then_ShouldReturnWaitingToStart_WhenActualStartDateIsInFuture()
    {
        // Arrange
        var query = _fixture.Create<GetLearnerStatusRequest>();
        var futureDate = DateTime.UtcNow.AddDays(1);
        MockStartDateInRepository(futureDate);
        _systemClockServiceMock.Setup(clock => clock.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Should().NotBeNull();
        result!.LearnerStatus.Should().Be(LearnerStatus.WaitingToStart);
    }

    [Test]
    public async Task Then_ShouldReturnInLearning_WhenActualStartDateIsInPast()
    {
        // Arrange
        var query = _fixture.Create<GetLearnerStatusRequest>();
        var pastDate = DateTime.UtcNow.AddDays(-1);
        MockStartDateInRepository(pastDate);
        _systemClockServiceMock.Setup(clock => clock.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Should().NotBeNull();
        result!.LearnerStatus.Should().Be(LearnerStatus.InLearning);
    }

    private void MockStartDateInRepository(DateTime? startDate = null)
    {
        ApprenticeshipStartDate? apprenticeshipStartDate = null;
        if (startDate.HasValue)
        {
            apprenticeshipStartDate = new ApprenticeshipStartDate { ActualStartDate = startDate.Value };
        }

        _apprenticeshipQueryRepositoryMock.Setup(repo => repo.GetStartDate(It.IsAny<Guid>()))
            .ReturnsAsync(apprenticeshipStartDate);
    }
}
