using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Command.CancelPendingStartDateChange;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.StartDateChangeControllerTests;

public class WhenCancelledStartDateChange
{
    private Fixture _fixture;
    private Mock<IQueryDispatcher> _queryDispatcher;
    private Mock<ICommandDispatcher> _commandDispatcher;
    private StartDateChangeController _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _queryDispatcher = new Mock<IQueryDispatcher>();
        _commandDispatcher = new Mock<ICommandDispatcher>();
        var logger = Mock.Of<ILogger<StartDateChangeController>>();
        _sut = new StartDateChangeController(_queryDispatcher.Object, _commandDispatcher.Object, logger);
    }

    [Test]
    public async Task ThenCancelStartDateChangeCommandIsSent()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();

        // Act
        var result = await _sut.CancelPendingStartDateChange(apprenticeshipKey);

        // Assert
        _commandDispatcher.Verify(x => x.Send(
            It.Is<CancelPendingStartDateChangeRequest>(r => r.ApprenticeshipKey == apprenticeshipKey),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ThenOkResultIsReturned()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();

        // Act
        var result = await _sut.CancelPendingStartDateChange(apprenticeshipKey);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}