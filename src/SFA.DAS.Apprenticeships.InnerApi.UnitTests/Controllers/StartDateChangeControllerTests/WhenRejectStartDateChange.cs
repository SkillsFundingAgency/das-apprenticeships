using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.RejectStartDateChange;
using SFA.DAS.Apprenticeships.InnerApi.Controllers;
using SFA.DAS.Apprenticeships.InnerApi.Requests;
using SFA.DAS.Apprenticeships.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.StartDateChangeControllerTests;

public class WhenRejectStartDateChange
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
    public async Task ThenRejectStartDateChangeCommandIsSent()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var reason = _fixture.Create<string>();
        var request = new RejectStartDateChangeRequest { Reason = reason };

        // Act
        var result = await _sut.RejectStartDateChange(apprenticeshipKey, request);

        // Assert
        _commandDispatcher.Verify(x => x.Send(
            It.Is<RejectStartDateChangeCommand>(r => r.ApprenticeshipKey == apprenticeshipKey && r.Reason == reason),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ThenOkResultIsReturned()
    {
        // Arrange
        var apprenticeshipKey = _fixture.Create<Guid>();
        var reason = _fixture.Create<string>();
        var request = new RejectStartDateChangeRequest { Reason = reason };

        // Act
        var result = await _sut.RejectStartDateChange(apprenticeshipKey, request);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}