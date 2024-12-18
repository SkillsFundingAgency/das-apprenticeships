using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Command;
using SFA.DAS.Apprenticeships.Command.WithdrawApprenticeship;
using SFA.DAS.Apprenticeships.Functions;

namespace SFA.DAS.Apprenticeships.Functions.UnitTests;

[TestFixture]
public class WithdrawApprenticeshipFunctionTests
{
    private Mock<ICommandDispatcher> _commandDispatcherMock;
    private Mock<ILogger> _loggerMock;
    private WithdrawApprenticeshipFunction _function;

    [SetUp]
    public void SetUp()
    {
        _commandDispatcherMock = new Mock<ICommandDispatcher>();
        _loggerMock = new Mock<ILogger>();
        _function = new WithdrawApprenticeshipFunction(_commandDispatcherMock.Object);
    }

    [Test]
    public async Task Run_ShouldReturnOkResult_WhenCommandIsSuccessful()
    {
        // Arrange
        var command = new WithdrawApprenticeshipCommand();
        var response = new WithdrawApprenticeshipResponse { IsSuccess = true };
        _commandDispatcherMock
            .Setup(x => x.Send<WithdrawApprenticeshipCommand, WithdrawApprenticeshipResponse>(It.IsAny<WithdrawApprenticeshipCommand>(), default))
            .ReturnsAsync(response);

        var request = CreateHttpRequest(command);

        // Act
        var result = await _function.Run(request, _loggerMock.Object);

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
        var response = new WithdrawApprenticeshipResponse { IsSuccess = false, Message = "Error" };
        _commandDispatcherMock
            .Setup(x => x.Send<WithdrawApprenticeshipCommand, WithdrawApprenticeshipResponse>(It.IsAny<WithdrawApprenticeshipCommand>(), default))
            .ReturnsAsync(response);

        var request = CreateHttpRequest(command);

        // Act
        var result = await _function.Run(request, _loggerMock.Object);

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
        var result = await _function.Run(request, _loggerMock.Object);

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

