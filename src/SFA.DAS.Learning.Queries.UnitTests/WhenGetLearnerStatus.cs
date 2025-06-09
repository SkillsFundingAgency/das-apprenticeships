using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.DataTransferObjects;
using SFA.DAS.Learning.Domain;
using SFA.DAS.Learning.Domain.Repositories;
using SFA.DAS.Learning.Infrastructure.Services;
using SFA.DAS.Learning.Queries.GetLearnerStatus;
using SFA.DAS.Learning.Types;

namespace SFA.DAS.Learning.Queries.UnitTests;


[TestFixture]
public class WhenGetLearnerStatus
{
    private Mock<ILearningQueryRepository> _apprenticeshipQueryRepositoryMock;
    private Mock<ISystemClockService> _systemClockServiceMock;
    private Mock<ILogger<GetLearnerStatusQueryHandler>> _loggerMock;
    private GetLearnerStatusQueryHandler _handler;
    private Fixture _fixture = new Fixture();

    [SetUp]
    public void SetUp()
    {
        _apprenticeshipQueryRepositoryMock = new Mock<ILearningQueryRepository>();
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

    [TestCase(31, 12, 2024, 31, 12, 2024)]
    [TestCase(30, 12, 2024, 30, 11, 2024)]
    public async Task Then_ShouldReturnWithdrawn_WhenDomainLearnerStatusIsWithdrawn(int lastDayOfLearningDay, int lastDayOfLearningMonth, int lastDayOfLearningYear, int lastCensusDateDay, int lastCensusDateMonth, int lastCensusDateYear)
    {
        // Arrange
        var query = _fixture.Create<GetLearnerStatusRequest>();
        var pastDate = DateTime.UtcNow.AddDays(-2);
        var lastDayOfLearning = new DateTime(lastDayOfLearningYear, lastDayOfLearningMonth, lastDayOfLearningDay);
        var withdrawalChangedDate = DateTime.UtcNow;
        var withdrawalReason = _fixture.Create<string>();
        MockStartDateInRepository(pastDate);
        _apprenticeshipQueryRepositoryMock.Setup(repo => repo.GetLearnerStatus(It.IsAny<Guid>()))
            .ReturnsAsync(new LearnerStatusDetails { LearnerStatus = Learning.Domain.Apprenticeship.LearnerStatus.Withdrawn, WithdrawalChangedDate = withdrawalChangedDate, WithdrawalReason = withdrawalReason, LastDayOfLearning = lastDayOfLearning});
        _systemClockServiceMock.Setup(clock => clock.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Should().NotBeNull();
        result!.LearnerStatus.Should().Be(LearnerStatus.Withdrawn);
        result.WithdrawalChangedDate.Should().Be(withdrawalChangedDate);
        result.WithdrawalReason.Should().Be(withdrawalReason);
        result.LastCensusDateOfLearning?.Date.Should().Be(new DateTime(lastCensusDateYear, lastCensusDateMonth, lastCensusDateDay));
        result.LastDayOfLearning?.Date.Should().Be(lastDayOfLearning);
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
