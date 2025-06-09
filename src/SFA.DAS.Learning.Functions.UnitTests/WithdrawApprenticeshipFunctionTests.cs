using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.Command.WithdrawApprenticeship;
using SFA.DAS.Learning.Domain;

namespace SFA.DAS.Learning.Functions.UnitTests;

[TestFixture]
public class WithdrawApprenticeshipFunctionTests
{
    private Mock<ICommandDispatcher> _commandDispatcherMock;
    private Mock<ILogger<WithdrawApprenticeshipFunction>> _loggerMock;
    private WithdrawApprenticeshipFunction _function;

    [SetUp]
    public void SetUp()
    {
        _commandDispatcherMock = new Mock<ICommandDispatcher>();
        _loggerMock = new Mock<ILogger<WithdrawApprenticeshipFunction>>();
        _function = new WithdrawApprenticeshipFunction(_commandDispatcherMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Run_ShouldReturnOkResult_WhenCommandIsSuccessful()
    {
        // Arrange
        var command = new WithdrawApprenticeshipCommand();
        var response = Outcome.Success();
        _commandDispatcherMock
            .Setup(x => x.Send<WithdrawApprenticeshipCommand, Outcome>(It.IsAny<WithdrawApprenticeshipCommand>(), default))
            .ReturnsAsync(response);

        var request = CreateHttpRequest(command);

        // Act
        var result = await _function.Run(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().Be("Completed");
    }

    [Test]
    public async Task Run_ShouldReturnBadRequest_WhenCommandFails()
    {
        // Arrange
        var command = new WithdrawApprenticeshipCommand();
        var response = Outcome.Fail("Error");
        _commandDispatcherMock
            .Setup(x => x.Send<WithdrawApprenticeshipCommand, Outcome>(It.IsAny<WithdrawApprenticeshipCommand>(), default))
            .ReturnsAsync(response);

        var request = CreateHttpRequest(command);

        // Act
        var result = await _function.Run(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Value.Should().Be("Error");
    }

    [Test]
    public async Task Run_ShouldReturnBadRequest_WhenRequestBodyIsInvalid()
    {
        // Arrange
        var request = new DefaultHttpContext().Request;
        request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Invalid body"));

        // Act
        var result = await _function.Run(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Value.Should().Be("Invalid request body");
    }

    private static Microsoft.AspNetCore.Http.HttpRequest CreateHttpRequest(object body)
    {
        var context = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        var request = context.Request;
        request.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(body)));
        request.ContentType = "application/json";
        return request;
    }
}

