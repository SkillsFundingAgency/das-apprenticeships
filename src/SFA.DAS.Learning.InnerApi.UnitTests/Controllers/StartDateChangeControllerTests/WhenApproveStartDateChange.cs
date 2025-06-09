using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Command.ApproveStartDateChange;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.InnerApi.Requests;
using SFA.DAS.Learning.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.StartDateChangeControllerTests;

public class WhenApproveStartDateChange
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
    public async Task ThenPendingStartDateChangeIsReturned()
    {
        var apprenticeshipKey = _fixture.Create<Guid>();
        var request = _fixture.Create<ApproveStartDateChangeRequest>();

        var result = await _sut.ApproveStartDateChange(apprenticeshipKey, request);

        _commandDispatcher.Verify(x => x.Send(
            It.Is<ApproveStartDateChangeCommand>(r => r.ApprenticeshipKey == apprenticeshipKey && r.UserId == request.UserId), 
            It.IsAny<CancellationToken>()), Times.Once);
        result.Should().BeOfType<OkResult>();
    }
}