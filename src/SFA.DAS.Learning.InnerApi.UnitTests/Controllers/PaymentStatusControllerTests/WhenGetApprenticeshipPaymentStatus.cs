using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Learning.Command;
using SFA.DAS.Learning.InnerApi.Controllers;
using SFA.DAS.Learning.Queries;
using SFA.DAS.Learning.Queries.GetApprenticeshipPaymentStatus;

namespace SFA.DAS.Learning.InnerApi.UnitTests.Controllers.PaymentStatusControllerTests;

public class WhenGetApprenticeshipPaymentStatus
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
    }

    [Test]
    public async Task ThenApprenticeshipPaymentStatusIsReturned()
    {
        var apprenticeshipKey = _fixture.Create<Guid>();
        var expectedResult = _fixture.Create<GetLearningPaymentStatusResponse>();

        _queryDispatcher
            .Setup(x => x.Send<GetLearningPaymentStatusRequest, GetLearningPaymentStatusResponse>(It.Is<GetLearningPaymentStatusRequest>(r => r.LearningKey == apprenticeshipKey)))
            .ReturnsAsync(expectedResult);

        var result = await _sut.GetLearningPaymentStatus(apprenticeshipKey);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().Be(expectedResult);
    }

    [Test]
    public async Task ThenNotFoundIsReturnedWhenNoRecordExists()
    {
        var apprenticeshipKey = _fixture.Create<Guid>();

        _queryDispatcher
            .Setup(x => x.Send<GetLearningPaymentStatusRequest, GetLearningPaymentStatusResponse>(It.Is<GetLearningPaymentStatusRequest>(r => r.LearningKey == apprenticeshipKey)))
            .ReturnsAsync((GetLearningPaymentStatusResponse)null!);

        var result = await _sut.GetLearningPaymentStatus(apprenticeshipKey);

        result.Should().BeOfType<NotFoundResult>();
    }
}
