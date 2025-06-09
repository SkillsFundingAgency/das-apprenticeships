using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Command.SetPaymentsFrozen;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.Queries;

namespace SFA.DAS.Apprenticeships.InnerApi.UnitTests.Controllers.PaymentStatusControllerTests;

public class WhenUnfreezePaymentStatus
{
    private Fixture _fixture;
    private Mock<IQueryDispatcher> _queryDispatcher;
    private Mock<ICommandDispatcher> _commandDispatcher;
    private Mock<ILogger<PaymentStatusController>> _mockLogger;
    private PaymentStatusController _sut;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _queryDispatcher = new Mock<IQueryDispatcher>();
        _commandDispatcher = new Mock<ICommandDispatcher>();
        _mockLogger = new Mock<ILogger<PaymentStatusController>>();
        _sut = new PaymentStatusController(_queryDispatcher.Object, _commandDispatcher.Object, _mockLogger.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.Items).Returns(new Dictionary<object, object?> { { "UserId", "testUserId" } });
        _sut.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = mockHttpContext.Object };
    }

    [Test]
    public async Task And_CommandExecutes_Then_ReturnsOk()
    {
        //  Arrange
        _commandDispatcher.Setup(x => x.Send(It.IsAny<SetPaymentsFrozenCommand>(), It.IsAny<CancellationToken>()));

        //  Act
        var result = await _sut.UnfreezePaymentStatus(_fixture.Create<Guid>());

        //  Assert
        result.Should().BeOfType<OkResult>();
    }

    [Test]
    public async Task And_CommandThrows_Then_ReturnsBadRequest()
    {
        //  Arrange
        _commandDispatcher.Setup(x => x.Send(It.IsAny<SetPaymentsFrozenCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("Test"));

        //  Act
        var result = await _sut.UnfreezePaymentStatus(_fixture.Create<Guid>());

        //  Assert
        result.Should().BeOfType<BadRequestResult>();
    }
}
